using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ObterHistoricoProcessamentoHandler
    : IRequestHandler<ObterHistoricoProcessamentoQuery, Result<List<HistoricoProcessamentoResponse>>>
{
    private readonly IProcessamentoRepository _repo;

    public ObterHistoricoProcessamentoHandler(IProcessamentoRepository repo) => _repo = repo;

    public async Task<Result<List<HistoricoProcessamentoResponse>>> Handle(
        ObterHistoricoProcessamentoQuery query, CancellationToken ct)
    {
        if (!DateOnly.TryParseExact(query.Periodo, "yyyy-MM", out var periodo))
            return Result<List<HistoricoProcessamentoResponse>>.Failure(
                "VALIDACAO", "Período deve estar no formato YYYY-MM.");

        var historico = await _repo.GetHistoricoAsync(query.EmpresaId, periodo, ct);

        var responses = historico.Select(p => new HistoricoProcessamentoResponse(
            p.Id, p.Versao, p.Status.ToString(),
            p.DataInicio ?? p.CreatedAt, p.DataFim,
            p.ReabertoPor, p.MotivoReabertura, p.ReabertoEm)).ToList();

        return Result<List<HistoricoProcessamentoResponse>>.Success(responses);
    }
}
