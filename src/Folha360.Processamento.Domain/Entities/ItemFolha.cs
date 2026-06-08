using Folha360.Domain;

namespace Folha360.Processamento.Domain.Entities;

public class ItemFolha : BaseEntity
{
    public Guid ProcessamentoId { get; private set; }
    public Guid FuncionarioId { get; private set; }
    public Guid RubricaId { get; private set; }
    public FaseProcessamento Fase { get; private set; }
    public decimal BaseCalculo { get; private set; }
    public decimal Valor { get; private set; }
    public string? FormulaAplicada { get; private set; }
    public int Ordem { get; private set; }
    public DateTime DataCalculo { get; private set; }

    private ItemFolha()
    {
    }

    public ItemFolha(
        Guid processamentoId,
        Guid funcionarioId,
        Guid rubricaId,
        FaseProcessamento fase,
        decimal baseCalculo,
        decimal valor,
        string? formulaAplicada,
        int ordem)
    {
        Id = Guid.NewGuid();
        ProcessamentoId = processamentoId;
        FuncionarioId = funcionarioId;
        RubricaId = rubricaId;
        Fase = fase;
        BaseCalculo = baseCalculo;
        Valor = valor;
        FormulaAplicada = formulaAplicada;
        Ordem = ordem;
        DataCalculo = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
