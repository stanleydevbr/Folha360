using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade AfastamentoFuncionario — afastamentos do funcionário (doença, acidente, maternidade, etc.).
/// Schema: tenant.
/// </summary>
public class AfastamentoFuncionario : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public string Tipo { get; private set; } = null!;
    public DateOnly DataInicio { get; private set; }
    public DateOnly? DataFimPrevista { get; private set; }
    public DateOnly? DataFimEfetiva { get; private set; }
    public string? NumeroAtestadoCid { get; private set; }
    public string? Observacoes { get; private set; }

    private AfastamentoFuncionario()
        : base()
    {
    }

    public AfastamentoFuncionario(
        Guid funcionarioId,
        string tipo,
        DateOnly dataInicio,
        DateOnly? dataFimPrevista = null,
        DateOnly? dataFimEfetiva = null,
        string? numeroAtestadoCid = null,
        string? observacoes = null)
    {
        if (dataFimPrevista.HasValue && dataFimPrevista.Value < dataInicio)
        {
            throw new ArgumentException("Data fim prevista não pode ser anterior à data de início.");
        }

        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        Tipo = tipo;
        DataInicio = dataInicio;
        DataFimPrevista = dataFimPrevista;
        DataFimEfetiva = dataFimEfetiva;
        NumeroAtestadoCid = numeroAtestadoCid;
        Observacoes = observacoes;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegistrarRetorno(DateOnly dataFimEfetiva)
    {
        DataFimEfetiva = dataFimEfetiva;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly? dataFimPrevista = null,
        string? numeroAtestadoCid = null,
        string? observacoes = null)
    {
        if (dataFimPrevista.HasValue && dataFimPrevista.Value < DataInicio)
        {
            throw new ArgumentException("Data fim prevista não pode ser anterior à data de início.");
        }

        DataFimPrevista = dataFimPrevista;
        NumeroAtestadoCid = numeroAtestadoCid;
        Observacoes = observacoes;
        UpdatedAt = DateTime.UtcNow;
    }
}
