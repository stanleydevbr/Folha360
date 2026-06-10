using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ObterHoleriteHandler : IRequestHandler<ObterHoleriteQuery, Result<byte[]>>
{
    private readonly IHoleriteRepository _repo;

    public ObterHoleriteHandler(IHoleriteRepository repo) => _repo = repo;

    public async Task<Result<byte[]>> Handle(ObterHoleriteQuery query, CancellationToken ct)
    {
        var holerite = await _repo.GetByFuncionarioAsync(query.ProcessamentoId, query.FuncionarioId, ct);
        if (holerite is null)
            return Result<byte[]>.Failure("NAO_ENCONTRADO", "Holerite não encontrado.");

        // In production, this would download from MinIO using MinioKey
        return Result<byte[]>.Success(Array.Empty<byte>());
    }
}

public class ListarHoleritesHandler : IRequestHandler<ListarHoleritesQuery, Result<List<HoleriteResponse>>>
{
    private readonly IHoleriteRepository _repo;

    public ListarHoleritesHandler(IHoleriteRepository repo) => _repo = repo;

    public async Task<Result<List<HoleriteResponse>>> Handle(ListarHoleritesQuery query, CancellationToken ct)
    {
        var holerites = await _repo.GetByProcessamentoAsync(query.ProcessamentoId, ct);

        var responses = holerites.Select(h => new HoleriteResponse(
            h.FuncionarioId,
            string.Empty,
            h.MinioKey,
            h.DataGeracao,
            $"/api/folha/holerites/{query.ProcessamentoId}/{h.FuncionarioId}")).ToList();

        return Result<List<HoleriteResponse>>.Success(responses);
    }
}
