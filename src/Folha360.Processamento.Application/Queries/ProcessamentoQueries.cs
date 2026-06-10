using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using MediatR;

namespace Folha360.Processamento.Application.Queries;

public sealed record ObterProcessamentoQuery(Guid Id) : IRequest<Result<ProcessamentoResponse>>;

public sealed record ListarProcessamentosQuery : IRequest<PaginatedResult<ProcessamentoResponse>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public Guid? EmpresaId { get; init; }
    public string? Periodo { get; init; }
    public string? Status { get; init; }
    public string? TipoCalculo { get; init; }
}

public sealed record ObterItensFolhaQuery : IRequest<Result<List<ItemFolhaResponse>>>
{
    public Guid ProcessamentoId { get; init; }
    public Guid? FuncionarioId { get; init; }
}
