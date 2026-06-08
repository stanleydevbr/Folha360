using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ListarProcessamentosHandler : IRequestHandler<ListarProcessamentosQuery, PaginatedResult<ProcessamentoResponse>>
{
    private readonly IProcessamentoRepository _repo;

    public ListarProcessamentosHandler(IProcessamentoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<ProcessamentoResponse>> Handle(
        ListarProcessamentosQuery query, CancellationToken ct)
    {
        DateOnly? periodo = null;
        if (!string.IsNullOrWhiteSpace(query.Periodo) &&
            DateOnly.TryParseExact(query.Periodo, "yyyy-MM", out var parsed))
        {
            periodo = parsed;
        }

        StatusProcessamento? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<StatusProcessamento>(query.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        TipoCalculo? tipoCalculo = null;
        if (!string.IsNullOrWhiteSpace(query.TipoCalculo) &&
            Enum.TryParse<TipoCalculo>(query.TipoCalculo, true, out var parsedTipo))
        {
            tipoCalculo = parsedTipo;
        }

        var (items, totalCount) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.EmpresaId, periodo, status, tipoCalculo, ct);

        var responses = items.Select(p => new ProcessamentoResponse(
            p.Id, p.EmpresaId, $"{p.Periodo:yyyy-MM}", p.TipoCalculo.ToString(),
            p.Status.ToString(), p.Versao, p.TotalFuncionarios, p.FuncionariosProcessados,
            p.FuncionariosComErro, p.TotalVencimentos, p.TotalDescontos, p.TotalLiquido,
            p.DataInicio, p.DataFim, p.Erro));

        return PaginatedResult<ProcessamentoResponse>.Success(responses, query.Page, query.PageSize, totalCount);
    }
}
