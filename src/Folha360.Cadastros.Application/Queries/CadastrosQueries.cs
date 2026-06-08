using Folha360.Cadastros.Application.DTOs;
using MediatR;

namespace Folha360.Cadastros.Application.Queries;

// ============================
// Empresa
// ============================
public sealed record ObterEmpresaQuery(Guid Id) : IRequest<Result<EmpresaDto>>;
public sealed record ListarEmpresasQuery : IRequest<PaginatedResult<EmpresaDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public string? Cnpj { get; init; }
    public string? RazaoSocial { get; init; }
    public string? RegimeTributario { get; init; }
}

// ============================
// Funcionario
// ============================
public sealed record ObterFuncionarioQuery(Guid Id) : IRequest<Result<FuncionarioDto>>;
public sealed record ListarFuncionariosQuery : IRequest<PaginatedResult<FuncionarioDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public Guid? EmpresaId { get; init; }
    public string? Status { get; init; }
    public Guid? CargoId { get; init; }
    public Guid? LotacaoId { get; init; }
    public string? Nome { get; init; }
}

// ============================
// Cargo
// ============================
public sealed record ObterCargoQuery(Guid Id) : IRequest<Result<CargoDto>>;
public sealed record ListarCargosQuery : IRequest<PaginatedResult<CargoDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public Guid? EmpresaId { get; init; }
    public string? Nome { get; init; }
}

// ============================
// Rubrica
// ============================
public sealed record ObterRubricaQuery(Guid Id) : IRequest<Result<RubricaDto>>;
public sealed record ListarRubricasQuery : IRequest<PaginatedResult<RubricaDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public Guid? EmpresaId { get; init; }
    public string? Natureza { get; init; }
    public string? TipoEsocial { get; init; }
    public string? TipoCalculo { get; init; }
    public Guid? GrupoRubricaId { get; init; }
    public bool? Ativo { get; init; }
}

// ============================
// Lotacao
// ============================
public sealed record ObterLotacaoQuery(Guid Id) : IRequest<Result<LotacaoDto>>;
public sealed record ListarLotacoesQuery : IRequest<PaginatedResult<LotacaoDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public Guid? EmpresaId { get; init; }
    public string? Codigo { get; init; }
}

// ============================
// Subsistema de Rubricas (ADR-006)
// ============================

// --- GrupoRubrica ---
public sealed record ObterGrupoRubricaQuery(Guid Id) : IRequest<Result<GrupoRubricaDto>>;
public sealed record ListarGruposRubricaQuery : IRequest<PaginatedResult<GrupoRubricaDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? OrderBy { get; init; }
    public Guid? EmpresaId { get; init; }
    public string? Natureza { get; init; }
}

// --- RubricaComposicao ---
public sealed record ListarComposicaoQuery(Guid RubricaPrincipalId) : IRequest<Result<List<RubricaComposicaoDto>>>;

// --- RubricaFormula ---
public sealed record ObterRubricaFormulaQuery(Guid RubricaId) : IRequest<Result<RubricaFormulaDto>>;

// --- RubricaTabelaProgressiva ---
public sealed record ListarFaixasProgressivasQuery : IRequest<Result<List<RubricaTabelaProgressivaDto>>>
{
    public Guid RubricaId { get; init; }
    public int? AnoVigencia { get; init; }
}

// --- RubricaHistorico ---
public sealed record ListarHistoricoRubricaQuery : IRequest<PaginatedResult<RubricaHistoricoDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public Guid RubricaId { get; init; }
}

// --- RubricaIncidencia ---
public sealed record ListarIncidenciasQuery(Guid RubricaId) : IRequest<Result<List<RubricaIncidenciaDto>>>;
