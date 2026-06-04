using Folha360.Domain;
using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Folha360DbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Folha360DbContext>>();

        if (await context.Set<Usuario>().AnyAsync())
        {
            logger.LogInformation("Database already seeded. Skipping...");
            return;
        }

        logger.LogInformation("Seeding database...");

        // Criar tenant de demonstração
        var tenant = new Tenant(
            tenantId: "demo",
            schemaName: "tenant_demo",
            nome: "Empresa Demo Ltda",
            status: TenantStatus.Ativo);

        context.Set<Tenant>().Add(tenant);

        // Criar usuários mock para os 4 perfis
        var usuarios = new List<Usuario>
        {
            new(
                email: "admin@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Admin@123"),
                nome: "Administrador",
                perfil: PerfilAcesso.Admin,
                status: UsuarioStatus.Ativo),
            new(
                email: "operador@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Oper@123"),
                nome: "Operador",
                perfil: PerfilAcesso.Operador,
                status: UsuarioStatus.Ativo),
            new(
                email: "contador@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Cont@123"),
                nome: "Contador",
                perfil: PerfilAcesso.Contador,
                status: UsuarioStatus.Ativo),
            new(
                email: "consulta@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Cons@123"),
                nome: "Consulta",
                perfil: PerfilAcesso.Consulta,
                status: UsuarioStatus.Ativo),
        };

        context.Set<Usuario>().AddRange(usuarios);
        await context.SaveChangesAsync();

        logger.LogInformation("Database seeded successfully with {UserCount} users and 1 tenant", usuarios.Count);
    }
}
