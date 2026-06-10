using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaFormula : BaseEntity
{
    public Guid RubricaId { get; private set; }
    public string Expressao { get; private set; } = null!;
    public string? Parametros { get; private set; }
    public string? DescricaoFormal { get; private set; }
    public int Versao { get; private set; } = 1;
    public Rubrica Rubrica { get; private set; } = null!;

    private RubricaFormula()
    {
    }

    public RubricaFormula(Guid rubricaId, string expressao, string? parametros = null, string? descricaoFormal = null)
    {
        Id = Guid.NewGuid();
        RubricaId = rubricaId;
        Expressao = expressao;
        Parametros = parametros;
        DescricaoFormal = descricaoFormal;
        Versao = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string expressao, string? parametros = null, string? descricaoFormal = null)
    {
        Expressao = expressao;
        Parametros = parametros ?? Parametros;
        DescricaoFormal = descricaoFormal ?? DescricaoFormal;
        Versao++;
        UpdatedAt = DateTime.UtcNow;
    }
}
