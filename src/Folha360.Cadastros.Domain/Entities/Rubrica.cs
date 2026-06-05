using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Rubrica — vencimentos e descontos compatíveis com Tabela 03 do e-Social (S-1010).
/// Schema: tenant.
/// </summary>
public class Rubrica : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string Natureza { get; private set; } = null!;
    public string? TipoEsocial { get; private set; }
    public bool IncideInss { get; private set; }
    public bool IncideIrrf { get; private set; }
    public bool IncideFgts { get; private set; }
    public bool IncideContribuicaoSindical { get; private set; }
    public bool IncideDecimoTerceiro { get; private set; }
    public bool IncideFerias { get; private set; }
    public bool IncideAvisoPrevio { get; private set; }
    public string? FormulaCalculo { get; private set; }
    public int OrdemExibicao { get; private set; }

#pragma warning disable CS8618
    private Rubrica()
    {
    }
#pragma warning restore CS8618

    public Rubrica(
        Guid empresaId,
        string codigo,
        string descricao,
        string natureza,
        string? tipoEsocial = null,
        bool incideInss = false,
        bool incideIrrf = false,
        bool incideFgts = false,
        bool incideContribuicaoSindical = false,
        bool incideDecimoTerceiro = false,
        bool incideFerias = false,
        bool incideAvisoPrevio = false,
        string? formulaCalculo = null,
        int ordemExibicao = 0)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Descricao = descricao;
        Natureza = natureza;
        TipoEsocial = tipoEsocial;
        IncideInss = incideInss;
        IncideIrrf = incideIrrf;
        IncideFgts = incideFgts;
        IncideContribuicaoSindical = incideContribuicaoSindical;
        IncideDecimoTerceiro = incideDecimoTerceiro;
        IncideFerias = incideFerias;
        IncideAvisoPrevio = incideAvisoPrevio;
        FormulaCalculo = formulaCalculo;
        OrdemExibicao = ordemExibicao;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        string natureza,
        string? tipoEsocial = null,
        bool? incideInss = null,
        bool? incideIrrf = null,
        bool? incideFgts = null,
        bool? incideContribuicaoSindical = null,
        bool? incideDecimoTerceiro = null,
        bool? incideFerias = null,
        bool? incideAvisoPrevio = null,
        string? formulaCalculo = null,
        int? ordemExibicao = null)
    {
        Descricao = descricao;
        Natureza = natureza;
        TipoEsocial = tipoEsocial ?? TipoEsocial;
        IncideInss = incideInss ?? IncideInss;
        IncideIrrf = incideIrrf ?? IncideIrrf;
        IncideFgts = incideFgts ?? IncideFgts;
        IncideContribuicaoSindical = incideContribuicaoSindical ?? IncideContribuicaoSindical;
        IncideDecimoTerceiro = incideDecimoTerceiro ?? IncideDecimoTerceiro;
        IncideFerias = incideFerias ?? IncideFerias;
        IncideAvisoPrevio = incideAvisoPrevio ?? IncideAvisoPrevio;
        FormulaCalculo = formulaCalculo ?? FormulaCalculo;
        OrdemExibicao = ordemExibicao ?? OrdemExibicao;
        UpdatedAt = DateTime.UtcNow;
    }
}
