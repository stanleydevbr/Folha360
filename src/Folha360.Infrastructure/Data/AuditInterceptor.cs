using System.Text.Json;
using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Folha360.Infrastructure.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext? _tenantContext;

    public AuditInterceptor(ITenantContext? tenantContext = null)
    {
        _tenantContext = tenantContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = new List<AuditLogEntry>();
        var changedBy = _tenantContext?.TenantId ?? "system";
        var schemaName = _tenantContext?.SchemaName ?? "public";

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLogEntry || entry.Entity is Tenant)
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
            context.Set<AuditLogEntry>().AddRange(auditEntries);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
