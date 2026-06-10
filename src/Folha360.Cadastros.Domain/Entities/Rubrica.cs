using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Rubrica — vencimentos e descontos compatíveis com Tabela 03 do e-Social (S-1010).
/// Schema: tenant.
/// Atualização (Junho 2026): Expandida de 15 para 30+ colunas conforme ADR-006.
/// </summary>
public class Rubrica : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public Guid? GrupoRubricaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string? DescricaoAbreviada { get; private set; }
    public string Natureza { get; private set; } = null!;
    public string? TipoEsocial { get; private set; }
    public bool EnviarEsocial { get; private set; } = true;
    public bool IncideInss { get; private set; }
    public bool IncideIrrf { get; private set; }
    public bool IncideFgts { get; private set; }
    public bool IncideContribuicaoSindical { get; private set; }
    public bool IncideDecimoTerceiro { get; private set; }
    public bool IncideFerias { get; private set; }
    public bool IncideAvisoPrevio { get; private set; }
    public bool IncideRescisao { get; private set; }
    public bool IncideDissidio { get; private set; }
    public bool IncideSalarioMaternidade { get; private set; }
    public bool IncideAuxilioDoenca { get; private set; }
    public bool IncideAdiantamento { get; private set; }
    public string TipoCalculo { get; private set; } = "VALOR_FIXO";
    public string? FormulaCalculo { get; private set; }
    public decimal? ValorFixo { get; private set; }
    public decimal? Percentual { get; private set; }
    public Guid? RubricaBaseId { get; private set; }
    public int OrdemCalculo { get; private set; }
    public int OrdemExibicao { get; private set; }
    public int? PrioridadeDesconto { get; private set; }
    public decimal? TetoMaximo { get; private set; }
    public decimal? PisoMinimo { get; private set; }
    public bool Ativo { get; private set; } = true;
    public DateTime? DataInicioVigencia { get; private set; }
    public DateTime? DataFimVigencia { get; private set; }
    public string? Observacao { get; private set; }

    // Navigation properties
    public GrupoRubrica? GrupoRubrica { get; private set; }
    public Rubrica? RubricaBase { get; private set; }
    public ICollection<RubricaComposicao> Composicoes { get; private set; } = new List<RubricaComposicao>();
    public RubricaFormula? Formula { get; private set; }
    public ICollection<RubricaIncidencia> Incidencias { get; private set; } = new List<RubricaIncidencia>();
    public ICollection<RubricaTabelaProgressiva> TabelasProgressivas { get; private set; } = new List<RubricaTabelaProgressiva>();
    public ICollection<RubricaHistorico> Historico { get; private set; } = new List<RubricaHistorico>();

    private Rubrica()
    {
    }

    public Rubrica(
        Guid empresaId,
        string codigo,
        string descricao,
        string natureza,
        string? tipoEsocial = null,
        string? descricaoAbreviada = null,
        bool enviarEsocial = true,
        bool incideInss = false,
        bool incideIrrf = false,
        bool incideFgts = false,
        bool incideContribuicaoSindical = false,
        bool incideDecimoTerceiro = false,
        bool incideFerias = false,
        bool incideAvisoPrevio = false,
        bool incideRescisao = false,
        bool incideDissidio = false,
        bool incideSalarioMaternidade = false,
        bool incideAuxilioDoenca = false,
        bool incideAdiantamento = false,
        string tipoCalculo = "VALOR_FIXO",
        string? formulaCalculo = null,
        decimal? valorFixo = null,
        decimal? percentual = null,
        Guid? rubricaBaseId = null,
        int ordemCalculo = 0,
        int ordemExibicao = 0,
        int? prioridadeDesconto = null,
        decimal? tetoMaximo = null,
        decimal? pisoMinimo = null,
        bool ativo = true,
        DateTime? dataInicioVigencia = null,
        DateTime? dataFimVigencia = null,
        string? observacao = null,
        Guid? grupoRubricaId = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Descricao = descricao;
        Natureza = natureza;
        TipoEsocial = tipoEsocial;
        DescricaoAbreviada = descricaoAbreviada;
        EnviarEsocial = enviarEsocial;
        IncideInss = incideInss;
        IncideIrrf = incideIrrf;
        IncideFgts = incideFgts;
        IncideContribuicaoSindical = incideContribuicaoSindical;
        IncideDecimoTerceiro = incideDecimoTerceiro;
        IncideFerias = incideFerias;
        IncideAvisoPrevio = incideAvisoPrevio;
        IncideRescisao = incideRescisao;
        IncideDissidio = incideDissidio;
        IncideSalarioMaternidade = incideSalarioMaternidade;
        IncideAuxilioDoenca = incideAuxilioDoenca;
        IncideAdiantamento = incideAdiantamento;
        TipoCalculo = tipoCalculo;
        FormulaCalculo = formulaCalculo;
        ValorFixo = valorFixo;
        Percentual = percentual;
        RubricaBaseId = rubricaBaseId;
        OrdemCalculo = ordemCalculo;
        OrdemExibicao = ordemExibicao;
        PrioridadeDesconto = prioridadeDesconto;
        TetoMaximo = tetoMaximo;
        PisoMinimo = pisoMinimo;
        Ativo = ativo;
        DataInicioVigencia = dataInicioVigencia;
        DataFimVigencia = dataFimVigencia;
        Observacao = observacao;
        GrupoRubricaId = grupoRubricaId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        string natureza,
        string? tipoEsocial = null,
        string? descricaoAbreviada = null,
        bool? enviarEsocial = null,
        bool? incideInss = null,
        bool? incideIrrf = null,
        bool? incideFgts = null,
        bool? incideContribuicaoSindical = null,
        bool? incideDecimoTerceiro = null,
        bool? incideFerias = null,
        bool? incideAvisoPrevio = null,
        bool? incideRescisao = null,
        bool? incideDissidio = null,
        bool? incideSalarioMaternidade = null,
        bool? incideAuxilioDoenca = null,
        bool? incideAdiantamento = null,
        string? tipoCalculo = null,
        string? formulaCalculo = null,
        decimal? valorFixo = null,
        decimal? percentual = null,
        Guid? rubricaBaseId = null,
        int? ordemCalculo = null,
        int? ordemExibicao = null,
        int? prioridadeDesconto = null,
        decimal? tetoMaximo = null,
        decimal? pisoMinimo = null,
        bool? ativo = null,
        DateTime? dataInicioVigencia = null,
        DateTime? dataFimVigencia = null,
        string? observacao = null,
        Guid? grupoRubricaId = null)
    {
        Descricao = descricao;
        Natureza = natureza;
        TipoEsocial = tipoEsocial ?? TipoEsocial;
        DescricaoAbreviada = descricaoAbreviada ?? DescricaoAbreviada;
        EnviarEsocial = enviarEsocial ?? EnviarEsocial;
        IncideInss = incideInss ?? IncideInss;
        IncideIrrf = incideIrrf ?? IncideIrrf;
        IncideFgts = incideFgts ?? IncideFgts;
        IncideContribuicaoSindical = incideContribuicaoSindical ?? IncideContribuicaoSindical;
        IncideDecimoTerceiro = incideDecimoTerceiro ?? IncideDecimoTerceiro;
        IncideFerias = incideFerias ?? IncideFerias;
        IncideAvisoPrevio = incideAvisoPrevio ?? IncideAvisoPrevio;
        IncideRescisao = incideRescisao ?? IncideRescisao;
        IncideDissidio = incideDissidio ?? IncideDissidio;
        IncideSalarioMaternidade = incideSalarioMaternidade ?? IncideSalarioMaternidade;
        IncideAuxilioDoenca = incideAuxilioDoenca ?? IncideAuxilioDoenca;
        IncideAdiantamento = incideAdiantamento ?? IncideAdiantamento;
        TipoCalculo = tipoCalculo ?? TipoCalculo;
        FormulaCalculo = formulaCalculo ?? FormulaCalculo;
        ValorFixo = valorFixo ?? ValorFixo;
        Percentual = percentual ?? Percentual;
        RubricaBaseId = rubricaBaseId ?? RubricaBaseId;
        OrdemCalculo = ordemCalculo ?? OrdemCalculo;
        OrdemExibicao = ordemExibicao ?? OrdemExibicao;
        PrioridadeDesconto = prioridadeDesconto ?? PrioridadeDesconto;
        TetoMaximo = tetoMaximo ?? TetoMaximo;
        PisoMinimo = pisoMinimo ?? PisoMinimo;
        Ativo = ativo ?? Ativo;
        DataInicioVigencia = dataInicioVigencia ?? DataInicioVigencia;
        DataFimVigencia = dataFimVigencia ?? DataFimVigencia;
        Observacao = observacao ?? Observacao;
        GrupoRubricaId = grupoRubricaId ?? GrupoRubricaId;
        UpdatedAt = DateTime.UtcNow;
    }
}
