using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class GerarGuiasHandler : IRequestHandler<GerarGuiasCommand, Result<List<GuiaRecolhimentoDto>>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public GerarGuiasHandler(
        IApuracaoFiscalRepository apuracaoRepo,
        IGuiaRecolhimentoRepository guiaRepo)
    {
        _apuracaoRepo = apuracaoRepo;
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<List<GuiaRecolhimentoDto>>> Handle(GerarGuiasCommand request, CancellationToken ct)
    {
        var apuracao = await _apuracaoRepo.GetByIdAsync(request.ApuracaoFiscalId, ct);
        if (apuracao == null)
        {
            return Result<List<GuiaRecolhimentoDto>>.Failure("NOT_FOUND", "Apuração não encontrada.");
        }

        var tipoGuia = apuracao.Tributo switch
        {
            Tributo.INSS => TipoGuia.GPS,
            Tributo.FGTS => TipoGuia.GRF,
            _ => TipoGuia.DARF,
        };

        var guia = new GuiaRecolhimento(
            apuracao.EmpresaId,
            apuracao.Periodo,
            apuracao.Id,
            tipoGuia,
            apuracao.Tributo,
            "0000",
            apuracao.ValorDevido,
            apuracao.DataVencimento);

        guia.Gerar($"{apuracao.EmpresaId}/{apuracao.Periodo:yyyy-MM}/{tipoGuia}_{apuracao.Tributo}.pdf");
        await _guiaRepo.AddAsync(guia, ct);

        var dto = new GuiaRecolhimentoDto(
            guia.Id,
            guia.TipoGuia.ToString(),
            guia.Tributo.ToString(),
            guia.Valor,
            guia.DataVencimento.ToString("yyyy-MM-dd"),
            guia.Status.ToString(),
            guia.MinioKey);

        return Result<List<GuiaRecolhimentoDto>>.Success(new List<GuiaRecolhimentoDto> { dto });
    }
}
