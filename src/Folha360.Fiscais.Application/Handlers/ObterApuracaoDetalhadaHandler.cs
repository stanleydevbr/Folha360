using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterApuracaoDetalhadaHandler : IRequestHandler<ObterApuracaoDetalhadaQuery, Result<List<ApuracaoFiscalDto>>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;

    public ObterApuracaoDetalhadaHandler(IApuracaoFiscalRepository apuracaoRepo)
    {
        _apuracaoRepo = apuracaoRepo;
    }

    public async Task<Result<List<ApuracaoFiscalDto>>> Handle(ObterApuracaoDetalhadaQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var apuracoes = await _apuracaoRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var dtos = apuracoes.Select(a => new ApuracaoFiscalDto(
            a.Id,
            a.EmpresaId,
            a.Periodo.ToString("yyyy-MM"),
            a.Tributo.ToString(),
            a.BaseCalculo,
            a.Aliquota,
            a.ValorDevido,
            a.Status.ToString(),
            a.DataVencimento.ToString("yyyy-MM-dd"))).ToList();

        return Result<List<ApuracaoFiscalDto>>.Success(dtos);
    }
}
