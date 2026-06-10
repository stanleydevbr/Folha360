using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ObterReaberturaStatusHandler
    : IRequestHandler<ObterReaberturaStatusQuery, Result<ReaberturaStatusResponse>>
{
    private readonly IProcessamentoRepository _repo;

    public ObterReaberturaStatusHandler(IProcessamentoRepository repo) => _repo = repo;

    public async Task<Result<ReaberturaStatusResponse>> Handle(
        ObterReaberturaStatusQuery query, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(query.ProcessamentoId, ct);
        if (p is null)
            return Result<ReaberturaStatusResponse>.Failure("NAO_ENCONTRADO", "Processamento não encontrado.");

        return Result<ReaberturaStatusResponse>.Success(new ReaberturaStatusResponse(
            p.Id,
            p.Status.ToString(),
            p.Status == Domain.StatusProcessamento.Reaberta ? "ReversaoConcluida" : "N/A",
            p.DataInicio ?? p.CreatedAt,
            p.Erro));
    }
}
