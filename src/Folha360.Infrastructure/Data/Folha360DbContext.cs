using Folha360.Domain.Entities;
using Folha360.Infrastructure.Data.Configurations.Base;
using Folha360.Infrastructure.Data.Configurations.Cadastros;
using Folha360.Infrastructure.Data.Configurations.Eventos;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Infrastructure.Data;

/// <summary>
/// Modelo canônico do Folha360 — contém TODAS as entidades de TODOS os módulos.
/// Este é o assembly DONO das migrations (Migrations Assembly Centralizado).
/// </summary>
public class Folha360DbContext : DbContext
{
    public Folha360DbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }

    // ============================
    // Tabelas base (schema public)
    // ============================
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    // ============================
    // Módulo F02 — Cadastros (schema public)
    // ============================
    public DbSet<Folha360.Cadastros.Domain.Entities.Empresa> Empresas => Set<Folha360.Cadastros.Domain.Entities.Empresa>();

    // ============================
    // Módulo F02 — Cadastros (schema tenant)
    // ============================
    public DbSet<Folha360.Cadastros.Domain.Entities.Funcionario> Funcionarios => Set<Folha360.Cadastros.Domain.Entities.Funcionario>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Cargo> Cargos => Set<Folha360.Cadastros.Domain.Entities.Cargo>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Rubrica> Rubricas => Set<Folha360.Cadastros.Domain.Entities.Rubrica>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Lotacao> Lotacoes => Set<Folha360.Cadastros.Domain.Entities.Lotacao>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Dependente> Dependentes => Set<Folha360.Cadastros.Domain.Entities.Dependente>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Documento> Documentos => Set<Folha360.Cadastros.Domain.Entities.Documento>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Sindicato> Sindicatos => Set<Folha360.Cadastros.Domain.Entities.Sindicato>();
    public DbSet<Folha360.Cadastros.Domain.Entities.Convenio> Convenios => Set<Folha360.Cadastros.Domain.Entities.Convenio>();
    public DbSet<Folha360.Cadastros.Domain.Entities.HorarioTrabalho> HorariosTrabalho => Set<Folha360.Cadastros.Domain.Entities.HorarioTrabalho>();

    // ============================
    // Módulo F03 — Eventos Trabalhistas (schema tenant)
    // ============================
    public DbSet<Folha360.Eventos.Domain.Entities.Admissao> Admissoes => Set<Folha360.Eventos.Domain.Entities.Admissao>();
    public DbSet<Folha360.Eventos.Domain.Entities.Ferias> Ferias => Set<Folha360.Eventos.Domain.Entities.Ferias>();
    public DbSet<Folha360.Eventos.Domain.Entities.Afastamento> Afastamentos => Set<Folha360.Eventos.Domain.Entities.Afastamento>();
    public DbSet<Folha360.Eventos.Domain.Entities.Desligamento> Desligamentos => Set<Folha360.Eventos.Domain.Entities.Desligamento>();
    public DbSet<Folha360.Eventos.Domain.Entities.AlteracaoContratual> AlteracoesContratuais => Set<Folha360.Eventos.Domain.Entities.AlteracaoContratual>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        // ============================
        // Configurações base
        // ============================
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        // ============================
        // Módulo F02 — Cadastros
        // ============================
        modelBuilder.ApplyConfiguration(new EmpresaConfiguration());
        modelBuilder.ApplyConfiguration(new FuncionarioConfiguration());
        modelBuilder.ApplyConfiguration(new CargoConfiguration());
        modelBuilder.ApplyConfiguration(new RubricaConfiguration());
        modelBuilder.ApplyConfiguration(new LotacaoConfiguration());
        modelBuilder.ApplyConfiguration(new DependenteConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentoConfiguration());
        modelBuilder.ApplyConfiguration(new SindicatoConfiguration());
        modelBuilder.ApplyConfiguration(new ConvenioConfiguration());
        modelBuilder.ApplyConfiguration(new HorarioTrabalhoConfiguration());

        // ============================
        // Módulo F03 — Eventos Trabalhistas
        // ============================
        modelBuilder.ApplyConfiguration(new AdmissaoConfiguration());
        modelBuilder.ApplyConfiguration(new FeriasConfiguration());
        modelBuilder.ApplyConfiguration(new AfastamentoConfiguration());
        modelBuilder.ApplyConfiguration(new DesligamentoConfiguration());
        modelBuilder.ApplyConfiguration(new AlteracaoContratualConfiguration());

        // ============================
        // Query Filter Global: Soft Delete
        // ============================
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Folha360.Domain.Abstractions.ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(Folha360.Domain.Abstractions.ISoftDeletable.DeletedAt));
                var nullCheck = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?)));
                var lambda = System.Linq.Expressions.Expression.Lambda(nullCheck, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
