using Folha360.Cadastros.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Data;

/// <summary>
/// DbContext de runtime do módulo de Cadastros (F02).
/// Herda de Folha360DbContext (modelo canônico) e expõe DbSets tipados para queries.
/// NÃO contém configurações de modelo — o modelo canônico está em Folha360DbContext.
/// NÃO contém migrations — as migrations são gerenciadas pelo Folha360.Infrastructure.
/// </summary>
public class CadastrosDbContext : Folha360DbContext
{
    public CadastrosDbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }

    // DbSets tipados para queries específicas do módulo
    public new DbSet<Empresa> Empresas => Set<Empresa>();
    public new DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public new DbSet<Cargo> Cargos => Set<Cargo>();
    public new DbSet<Rubrica> Rubricas => Set<Rubrica>();
    public new DbSet<Lotacao> Lotacoes => Set<Lotacao>();
    public new DbSet<Dependente> Dependentes => Set<Dependente>();
    public new DbSet<Documento> Documentos => Set<Documento>();
    public new DbSet<Sindicato> Sindicatos => Set<Sindicato>();
    public new DbSet<Convenio> Convenios => Set<Convenio>();
    public new DbSet<HorarioTrabalho> HorariosTrabalho => Set<HorarioTrabalho>();

    // Subsistema de Rubricas (ADR-006)
    public new DbSet<GrupoRubrica> GruposRubrica => Set<GrupoRubrica>();
    public new DbSet<RubricaComposicao> RubricasComposicao => Set<RubricaComposicao>();
    public new DbSet<RubricaFormula> RubricasFormula => Set<RubricaFormula>();
    public new DbSet<RubricaIncidencia> RubricasIncidencia => Set<RubricaIncidencia>();
    public new DbSet<RubricaTabelaProgressiva> RubricasTabelaProgressiva => Set<RubricaTabelaProgressiva>();
    public new DbSet<RubricaHistorico> RubricasHistorico => Set<RubricaHistorico>();
    public new DbSet<ProcessoAdministrativo> ProcessosAdministrativos => Set<ProcessoAdministrativo>();
    public new DbSet<RubricaProcesso> RubricasProcesso => Set<RubricaProcesso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Delega totalmente para o modelo canônico (Folha360DbContext)
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        InterceptSoftDelete();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        InterceptSoftDelete();
        return base.SaveChanges();
    }

    private void InterceptSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<Folha360.Domain.Abstractions.ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }
    }
}
