using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaTabelaProgressiva : BaseEntity
{
    public Guid RubricaId { get; private set; }
    public int AnoVigencia { get; private set; }
    public decimal FaixaDe { get; private set; }
    public decimal? FaixaAte { get; private set; }
    public decimal Aliquota { get; private set; }
    public decimal Deducao { get; private set; }
    public int Ordem { get; private set; }
    public Rubrica Rubrica { get; private set; } = null!;

    private RubricaTabelaProgressiva()
    {
    }

    public RubricaTabelaProgressiva(Guid rubricaId, int anoVigencia, decimal faixaDe, decimal? faixaAte, decimal aliquota, decimal deducao = 0, int ordem = 0)
    {
        Id = Guid.NewGuid();
        RubricaId = rubricaId;
        AnoVigencia = anoVigencia;
        FaixaDe = faixaDe;
        FaixaAte = faixaAte;
        Aliquota = aliquota;
        Deducao = deducao;
        Ordem = ordem;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
