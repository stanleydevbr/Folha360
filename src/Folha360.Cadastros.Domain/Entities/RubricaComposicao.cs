using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaComposicao : BaseEntity
{
    public Guid RubricaPrincipalId { get; private set; }
    public Guid RubricaComponenteId { get; private set; }
    public string Operador { get; private set; } = "+";
    public decimal? PercentualComposicao { get; private set; }
    public int Ordem { get; private set; }
    public bool Obrigatorio { get; private set; } = true;
    public Rubrica RubricaPrincipal { get; private set; } = null!;
    public Rubrica RubricaComponente { get; private set; } = null!;

    private RubricaComposicao()
    {
    }

    public RubricaComposicao(Guid rubricaPrincipalId, Guid rubricaComponenteId, string operador = "+", decimal? percentualComposicao = null, int ordem = 0, bool obrigatorio = true)
    {
        Id = Guid.NewGuid();
        RubricaPrincipalId = rubricaPrincipalId;
        RubricaComponenteId = rubricaComponenteId;
        Operador = operador;
        PercentualComposicao = percentualComposicao;
        Ordem = ordem;
        Obrigatorio = obrigatorio;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
