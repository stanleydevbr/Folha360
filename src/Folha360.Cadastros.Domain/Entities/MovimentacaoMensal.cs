using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade MovimentacaoMensal — rubricas específicas de um mês para o funcionário.
/// Sobrescreve a MovimentacaoFixa no mês correspondente.
/// Schema: tenant.
/// </summary>
public class MovimentacaoMensal : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid RubricaId { get; private set; }
    public string? Descricao { get; private set; }
    public string MesAno { get; private set; } = null!;
    public decimal? Quantidade { get; private set; }
    public decimal Valor { get; private set; }

    private MovimentacaoMensal()
        : base()
    {
    }

    public MovimentacaoMensal(
        Guid funcionarioId,
        Guid rubricaId,
        string mesAno,
        decimal valor,
        string? descricao = null,
        decimal? quantidade = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        RubricaId = rubricaId;
        MesAno = mesAno ?? throw new ArgumentNullException(nameof(mesAno));
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
