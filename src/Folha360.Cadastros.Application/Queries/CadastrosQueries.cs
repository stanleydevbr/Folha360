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
