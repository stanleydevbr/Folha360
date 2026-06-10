using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterApuracaoHandler : IRequestHandler<ObterApuracaoQuery, Result<ResumoApuracaoDto>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;

    public ObterApuracaoHandler(IApuracaoFiscalRepository apuracaoRepo)
    {
        _apuracaoRepo = apuracaoRepo;
    }

    public async Task<Result<ResumoApuracaoDto>> Handle(ObterApuracaoQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var apuracoes = await _apuracaoRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var totais = apuracoes.ToDictionary(
            a => a.Tributo.ToString(),
            a => a.ValorDevido);

        return Result<ResumoApuracaoDto>.Success(new ResumoApuracaoDto(
            request.EmpresaId,
            request.Periodo,
            Guid.Empty,
            apuracoes.Count > 0 ? "Concluido" : "Pendente",
            totais,
            new List<GuiaRecolhimentoDto>(),
            DateTime.UtcNow));
    }
}
