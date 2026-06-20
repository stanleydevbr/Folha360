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

    // Lookups (T11) — schema public
    public new DbSet<CboOcupacao> Cbos => Set<CboOcupacao>();
    public new DbSet<NaturezaJuridica> NaturezasJuridicas => Set<NaturezaJuridica>();
    public DbSet<MunicipioIBGE> Municipios => Set<MunicipioIBGE>();
    public DbSet<BancoFebraban> Bancos => Set<BancoFebraban>();

    // Expansão Empresa (T12)
    public new DbSet<ConfiguracaoBancariaEmpresa> ConfiguracoesBancariasEmpresa => Set<ConfiguracaoBancariaEmpresa>();
    public new DbSet<EnderecoEmpresa> EnderecosEmpresa => Set<EnderecoEmpresa>();
    public new DbSet<ContatoEmpresa> ContatosEmpresa => Set<ContatoEmpresa>();
    public new DbSet<ConfiguracaoGeral> ConfiguracoesGerais => Set<ConfiguracaoGeral>();
    public new DbSet<ConfiguracaoESocial> ConfiguracoesESocial => Set<ConfiguracaoESocial>();

    // Expansão Funcionário (T13)
    public new DbSet<DadosBancariosFuncionario> DadosBancariosFuncionarios => Set<DadosBancariosFuncionario>();
    public new DbSet<ContratoTrabalho> ContratosTrabalho => Set<ContratoTrabalho>();
    public new DbSet<RemuneracaoBeneficio> RemuneracoesBeneficios => Set<RemuneracaoBeneficio>();
    public new DbSet<AfastamentoFuncionario> Afastamentos => Set<AfastamentoFuncionario>();
    public new DbSet<InfoESocialFuncionario> InfosESocialFuncionario => Set<InfoESocialFuncionario>();
    public new DbSet<MovimentacaoFixa> MovimentacoesFixas => Set<MovimentacaoFixa>();
    public new DbSet<MovimentacaoMensal> MovimentacoesMensais => Set<MovimentacaoMensal>();

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
