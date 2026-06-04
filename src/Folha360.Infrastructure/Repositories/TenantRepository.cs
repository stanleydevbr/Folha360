using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Infrastructure.Repositories;

#pragma warning disable EF1002 // SQL injection warning suppressed - schema names are sanitized

public class TenantRepository : ITenantRepository
{
    private readonly Folha360DbContext _context;

    public TenantRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByIdAsync(string tenantId, CancellationToken ct = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.TenantId == tenantId, ct);
    }

    public async Task<string> CreateTenantSchemaAsync(string tenantId, CancellationToken ct = default)
    {
        var schemaName = SanitizeSchemaName($"tenant_{tenantId}");

        // Verificar se o schema já existe
        var exists = await _context.Database
            .SqlQueryRaw<bool>(
                $"SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = '{schemaName}')")
            .FirstOrDefaultAsync(ct);

        if (!exists)
        {
            // Clonar o schema template para o novo tenant
            await _context.Database.ExecuteSqlRawAsync(
                $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"", ct);

            // Copiar estrutura das tabelas do template_tenant
            var tables = await _context.Database
                .SqlQueryRaw<string>(
                    "SELECT tablename FROM pg_tables WHERE schemaname = 'template_tenant'")
                .ToListAsync(ct);

            foreach (var table in tables)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    $"CREATE TABLE \"{schemaName}\".\"{table}\" (LIKE \"template_tenant\".\"{table}\" INCLUDING ALL)", ct);
            }
        }

        return schemaName;
    }

    private static string SanitizeSchemaName(string name)
    {
        // Apenas permite caracteres alfanuméricos e underscore
        var sanitized = new System.Text.StringBuilder();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                sanitized.Append(c);
            else
                sanitized.Append('_');
        }
        return sanitized.ToString();
    }
}
