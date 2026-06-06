using Folha360.Domain.Abstractions;

namespace Folha360.Infrastructure.Data;

public static class DbContextExtensions
{
    /// <summary>
    /// Obtém o ITenantContext. Em operações fora do escopo HTTP (ex: migrations, seed),
    /// retorna null. O tenant context é gerenciado pelo TenantResolutionMiddleware.
    /// </summary>
    public static ITenantContext? GetTenantContext(this Folha360DbContext context)
    {
        // O tenant context é injetado via middleware, não via DbContext.
        // Para operações que precisam do tenant, use ITenantContext diretamente via DI.
        return null;
    }
}
