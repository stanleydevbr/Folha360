using Folha360.Application;
using Folha360.Eventos.Application.DTOs;
using MediatR;

namespace Folha360.Eventos.Application.Queries;

// ============================
// Admissao
// ============================
public sealed record ObterAdmissaoQuery(Guid Id) : IRequest<Result<AdmissaoDto>>;

public sealed record ListarAdmissoesQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? FuncionarioId = null) : IRequest<PaginatedResult<AdmissaoDto>>;

// ============================
// Ferias
// ============================
public sealed record ObterFeriasQuery(Guid Id) : IRequest<Result<FeriasDto>>;

public sealed record ListarFeriasQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? FuncionarioId = null) : IRequest<PaginatedResult<FeriasDto>>;

// ============================
// Afastamento
// ============================
public sealed record ObterAfastamentoQuery(Guid Id) : IRequest<Result<AfastamentoDto>>;

public sealed record ListarAfastamentosQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? FuncionarioId = null) : IRequest<PaginatedResult<AfastamentoDto>>;

// ============================
// Desligamento
// ============================
public sealed record ObterDesligamentoQuery(Guid Id) : IRequest<Result<DesligamentoDto>>;

public sealed record ListarDesligamentosQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? FuncionarioId = null) : IRequest<PaginatedResult<DesligamentoDto>>;

// ============================
// AlteracaoContratual
// ============================
public sealed record ObterAlteracaoContratualQuery(Guid Id) : IRequest<Result<AlteracaoContratualDto>>;

public sealed record ListarAlteracoesContratuaisQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? FuncionarioId = null) : IRequest<PaginatedResult<AlteracaoContratualDto>>;

// ============================
// Agregado
// ============================
public sealed record ListarEventosFuncionarioQuery(Guid FuncionarioId) : IRequest<Result<EventosFuncionarioDto>>;
