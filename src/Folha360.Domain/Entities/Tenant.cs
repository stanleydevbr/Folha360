namespace Folha360.Domain.Entities;

public sealed class Tenant : BaseEntity
{
    public string TenantId { get; private set; } = string.Empty;
    public string SchemaName { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public TenantStatus Status { get; private set; }
    public new DateTime CreatedAt { get; private set; }

    private Tenant()
    {
    }

    public Tenant(string tenantId, string schemaName, string nome, TenantStatus status)
    {
        TenantId = tenantId;
        SchemaName = schemaName;
        Nome = nome;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
}
