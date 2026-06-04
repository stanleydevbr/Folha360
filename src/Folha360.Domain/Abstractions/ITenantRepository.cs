using Folha360.Domain.Entities;

namespace Folha360.Domain.Abstractions;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(string tenantId, CancellationToken ct = default);
    Task<string> CreateTenantSchemaAsync(string tenantId, CancellationToken ct = default);
}
