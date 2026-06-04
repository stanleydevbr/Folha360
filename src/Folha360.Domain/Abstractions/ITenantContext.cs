namespace Folha360.Domain.Abstractions;

public interface ITenantContext
{
    string TenantId { get; }
    string SchemaName { get; }
}
