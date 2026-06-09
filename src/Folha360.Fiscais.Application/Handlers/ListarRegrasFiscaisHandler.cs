using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ListarRegrasFiscaisHandler : IRequestHandler<Queries.ListarRegrasFiscaisQuery, Result<List<RegraFiscalDto>>>
{
    private readonly IRegraFiscalRepository _regraRepo;

    public ListarRegrasFiscaisHandler(IRegraFiscalRepository regraRepo)
    {
        _regraRepo = regraRepo;
    }

    public async Task<Result<List<RegraFiscalDto>>> Handle(Queries.ListarRegrasFiscaisQuery request, CancellationToken ct)
    {
        var regras = await _regraRepo.GetAllAsync(ct);

        var dtos = regras.Select(r => new RegraFiscalDto(
            r.Id,
            r.Tributo.ToString(),
            r.Versao,
            r.VigenciaInicio.ToString("yyyy-MM-dd"),
            r.VigenciaFim?.ToString("yyyy-MM-dd"),
            r.CodigoReceita,
            r.Ativo)).ToList();

        return Result<List<RegraFiscalDto>>.Success(dtos);
    }
}
