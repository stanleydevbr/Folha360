using Folha360.Domain.Abstractions;

namespace Folha360.Infrastructure.Data;

public static class DbContextExtensions
{
    public static ITenantContext? GetTenantContext(this Folha360DbContext context)
    {
        return context.GetTenantContext();
    }
}
