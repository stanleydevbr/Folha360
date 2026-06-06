using Folha360.Application;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Domain;
using MediatR;

namespace Folha360.Eventos.Application.Commands;

// ============================
// Admissao
// ============================
public sealed record CriarAdmissaoCommand : IRequest<Result<AdmissaoDto>>
{
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataAdmissao { get; init; }
    public Guid CargoId { get; init; }
    public decimal SalarioInicial { get; init; }
    public TipoContrato TipoContrato { get; init; }
    public int? PeriodoExperienciaMeses { get; init; }
}

public sealed record AtualizarAdmissaoCommand : IRequest<Result<AdmissaoDto>>
{
    public Guid Id { get; init; }
    public DateOnly DataAdmissao { get; init; }
    public Guid CargoId { get; init; }
    public decimal SalarioInicial { get; init; }
    public TipoContrato TipoContrato { get; init; }
    public int? PeriodoExperienciaMeses { get; init; }
}

public sealed record ExcluirAdmissaoCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Ferias
// ============================
public sealed record CriarFeriasCommand : IRequest<Result<FeriasDto>>
{
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataInicio { get; init; }
    public int DiasGozo { get; init; }
    public DateOnly PeriodoAquisitivoInicio { get; init; }
    public DateOnly PeriodoAquisitivoFim { get; init; }
    public TipoFerias TipoFerias { get; init; }
}

public sealed record AtualizarFeriasCommand : IRequest<Result<FeriasDto>>
{
    public Guid Id { get; init; }
    public DateOnly DataInicio { get; init; }
    public int DiasGozo { get; init; }
    public DateOnly PeriodoAquisitivoInicio { get; init; }
    public DateOnly PeriodoAquisitivoFim { get; init; }
    public TipoFerias TipoFerias { get; init; }
}

public sealed record ExcluirFeriasCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Afastamento
// ============================
public sealed record CriarAfastamentoCommand : IRequest<Result<AfastamentoDto>>
{
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataInicio { get; init; }
    public DateOnly DataFimPrevista { get; init; }
    public TipoAfastamento TipoAfastamento { get; init; }
    public string? Cid { get; init; }
}

public sealed record AtualizarAfastamentoCommand : IRequest<Result<AfastamentoDto>>
{
    public Guid Id { get; init; }
    public DateOnly DataInicio { get; init; }
    public DateOnly DataFimPrevista { get; init; }
    public TipoAfastamento TipoAfastamento { get; init; }
    public DateOnly? DataFimEfetiva { get; init; }
    public string? Cid { get; init; }
}

public sealed record ExcluirAfastamentoCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Desligamento
// ============================
public sealed record CriarDesligamentoCommand : IRequest<Result<DesligamentoDto>>
{
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataDesligamento { get; init; }
    public MotivoDesligamento MotivoDesligamento { get; init; }
    public string? VerbasRescisorias { get; init; }
}

public sealed record AtualizarDesligamentoCommand : IRequest<Result<DesligamentoDto>>
{
    public Guid Id { get; init; }
    public DateOnly DataDesligamento { get; init; }
    public MotivoDesligamento MotivoDesligamento { get; init; }
    public string? VerbasRescisorias { get; init; }
}

public sealed record ExcluirDesligamentoCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// AlteracaoContratual
// ============================
public sealed record CriarAlteracaoContratualCommand : IRequest<Result<AlteracaoContratualDto>>
{
    public Guid FuncionarioId { get; init; }
    public Guid EmpresaId { get; init; }
    public DateOnly DataAlteracao { get; init; }
    public string? CamposAlterados { get; init; }
    public string? ValorAnterior { get; init; }
    public string? ValorNovo { get; init; }
}

public sealed record AtualizarAlteracaoContratualCommand : IRequest<Result<AlteracaoContratualDto>>
{
    public Guid Id { get; init; }
    public DateOnly DataAlteracao { get; init; }
    public string? CamposAlterados { get; init; }
    public string? ValorAnterior { get; init; }
    public string? ValorNovo { get; init; }
}

public sealed record ExcluirAlteracaoContratualCommand(Guid Id) : IRequest<Result<bool>>;
