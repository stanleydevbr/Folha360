using Folha360.Eventos.Domain;

namespace Folha360.Eventos.Application.DTOs;

public sealed record AdmissaoDto
{
    public Guid Id { get; init; }
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataAdmissao { get; init; }
    public Guid CargoId { get; init; }
    public decimal SalarioInicial { get; init; }
    public TipoContrato TipoContrato { get; init; }
    public int? PeriodoExperienciaMeses { get; init; }
    public string? XmlContent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record FeriasDto
{
    public Guid Id { get; init; }
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataInicio { get; init; }
    public int DiasGozo { get; init; }
    public DateOnly PeriodoAquisitivoInicio { get; init; }
    public DateOnly PeriodoAquisitivoFim { get; init; }
    public TipoFerias TipoFerias { get; init; }
    public string? XmlContent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record AfastamentoDto
{
    public Guid Id { get; init; }
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataInicio { get; init; }
    public DateOnly DataFimPrevista { get; init; }
    public DateOnly? DataFimEfetiva { get; init; }
    public TipoAfastamento TipoAfastamento { get; init; }
    public string? Cid { get; init; }
    public string? XmlContent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record DesligamentoDto
{
    public Guid Id { get; init; }
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataDesligamento { get; init; }
    public MotivoDesligamento MotivoDesligamento { get; init; }
    public string? VerbasRescisorias { get; init; }
    public string? XmlContent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record AlteracaoContratualDto
{
    public Guid Id { get; init; }
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataAlteracao { get; init; }
    public string? CamposAlterados { get; init; }
    public string? ValorAnterior { get; init; }
    public string? ValorNovo { get; init; }
    public string? XmlContent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record EventoFuncionarioItemDto
{
    public string TipoEvento { get; init; } = string.Empty;
    public Guid Id { get; init; }
    public DateOnly DataEvento { get; init; }
    public string Descricao { get; init; } = string.Empty;
}

public sealed record EventosFuncionarioDto
{
    public Guid FuncionarioId { get; init; }
    public List<EventoFuncionarioItemDto> Eventos { get; init; } = new();
}
