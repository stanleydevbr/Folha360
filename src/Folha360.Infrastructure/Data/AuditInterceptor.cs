using System.Runtime.CompilerServices;
using System.Text.Json;
using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Folha360.Infrastructure.Data;

/// <summary>
/// Interceptor de auditoria que captura alterações no change tracker e persiste
/// em AuditLogEntry APÓS o SaveChanges principal ser concluído com sucesso.
///
/// Fluxo:
///   1. SavingChangesAsync  → coleta dados de auditoria (SEM modificar o change tracker)
///   2. EF Core persiste as entidades originais
///   3. SavedChangesAsync    → adiciona entradas de auditoria e persiste em segundo SaveChangesAsync
///
/// Isso evita:
///   - Duplicação de auditoria em retentativas do ExecutionStrategy
///   - Aumento do payload do SaveChanges principal
///   - Modificação do change tracker durante a operação de save
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    // Armazena entradas de auditoria pendentes por instância de DbContext
    // ConditionalWeakTable usa weak references — não impede GC do DbContext
    private static readonly ConditionalWeakTable<DbContext, List<AuditLogEntry>> PendingAuditEntries = new();

    // Guard para evitar reentrância ao salvar auditoria (SavingChangesAsync → SavedChangesAsync)
    private static readonly ConditionalWeakTable<DbContext, object> AuditSaveGuard = new();

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        // Reentrância: se este SavingChangesAsync foi disparado pelo SaveChangesAsync
        // interno do SavedChangesAsync, não coletamos auditoria novamente.
        if (AuditSaveGuard.TryGetValue(context, out _))
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = new List<AuditLogEntry>();
        var changedBy = "system";
        var schemaName = "public";

        foreach (var entry in context.ChangeTracker.Entries())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entry.Entity is AuditLogEntry or Tenant)
                continue;

            var tableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name.ToLower();
            var recordId = entry.Property("Id")?.CurrentValue;

            if (recordId is not Guid guidId)
                continue;

            string? oldData = null;
            string? newData = null;
            string action;

            switch (entry.State)
            {
                case EntityState.Added:
                    action = "INSERT";
                    newData = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                    break;
                case EntityState.Modified:
                    action = "UPDATE";
                    oldData = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    newData = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
                    break;
                case EntityState.Deleted:
                    action = "DELETE";
                    oldData = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                    break;
                default:
                    continue;
            }

            auditEntries.Add(new AuditLogEntry(
                schemaName,
                tableName,
                guidId,
                action,
                oldData,
                newData,
                Guid.TryParse(changedBy, out var changedByGuid) ? changedByGuid : Guid.Empty));
        }

        if (auditEntries.Count > 0)
        {
            var entries = PendingAuditEntries.GetValue(context, _ => new List<AuditLogEntry>());
            entries.AddRange(auditEntries);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        // Reentrância: se este SavedChangesAsync foi disparado pelo SaveChangesAsync
        // interno que estamos usando para persistir a auditoria, não fazemos nada.
        if (AuditSaveGuard.TryGetValue(context, out _))
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        if (PendingAuditEntries.TryGetValue(context, out var auditEntries) && auditEntries.Count > 0)
        {
            // Ativa guard para que o SaveChangesAsync interno não dispare auditoria
            AuditSaveGuard.Add(context, null!);
            try
            {
                context.Set<AuditLogEntry>().AddRange(auditEntries);
                await context.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                AuditSaveGuard.Remove(context);
                PendingAuditEntries.Remove(context);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
