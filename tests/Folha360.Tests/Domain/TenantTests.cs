using Folha360.Domain;
using Folha360.Domain.Entities;

namespace Folha360.Tests.Domain;

[Trait("Category", "Unit")]
public class TenantTests
{
    [Fact]
    public void Tenant_ShouldCreateWithValidProperties()
    {
        var tenant = new Tenant("demo", "tenant_demo", "Empresa Demo Ltda", TenantStatus.Ativo);

        Assert.Equal("demo", tenant.TenantId);
        Assert.Equal("tenant_demo", tenant.SchemaName);
        Assert.Equal("Empresa Demo Ltda", tenant.Nome);
        Assert.Equal(TenantStatus.Ativo, tenant.Status);
        Assert.NotEqual(Guid.Empty, tenant.Id);
        Assert.NotEqual(default, tenant.CreatedAt);
    }

    [Fact]
    public void Tenant_ShouldCreateWithInactiveStatus()
    {
        var tenant = new Tenant("test", "tenant_test", "Test", TenantStatus.Inativo);
        Assert.Equal(TenantStatus.Inativo, tenant.Status);
    }
}
