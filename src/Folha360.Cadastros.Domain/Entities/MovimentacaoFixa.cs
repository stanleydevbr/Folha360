using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade MovimentacaoFixa — rubricas fixas mensais do funcionário.
/// Schema: tenant.
/// </summary>
public class MovimentacaoFixa : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid RubricaId { get; private set; }
    public string? Descricao { get; private set; }
    public decimal? Quantidade { get; private set; }
    public decimal Valor { get; private set; }

    private MovimentacaoFixa()
        : base()
    {
    }

    public MovimentacaoFixa(
        Guid funcionarioId,
        Guid rubricaId,
        decimal valor,
        string? descricao = null,
        decimal? quantidade = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        RubricaId = rubricaId;
        Valor = valor;
        Descricao = descricao;
        Quantidade = quantidade;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(decimal valor, string? descricao = null, decimal? quantidade = null)
    {
        Valor = valor;
        Descricao = descricao;
        Quantidade = quantidade;
        UpdatedAt = DateTime.UtcNow;
    }
}
