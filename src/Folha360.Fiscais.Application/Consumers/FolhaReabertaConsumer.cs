using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Processamento.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Consumers;

public class FolhaReabertaConsumer : IConsumer<FolhaReabertaEvent>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;
    private readonly ILogger<FolhaReabertaConsumer> _logger;

    public FolhaReabertaConsumer(
        IGuiaRecolhimentoRepository guiaRepo,
        ILogger<FolhaReabertaConsumer> logger)
    {
        _guiaRepo = guiaRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaReabertaEvent> context)
    {
        var evt = context.Message;
        _logger.LogWarning("Folha reaberta para empresa {EmpresaId} período {Periodo}. Invalidando guias fiscais...",
            evt.EmpresaId, evt.Periodo);

        var periodo = DateOnly.ParseExact(evt.Periodo + "-01", "yyyy-MM-dd");
        var guias = await _guiaRepo.GetByEmpresaPeriodoAsync(evt.EmpresaId, periodo, context.CancellationToken);

        foreach (var guia in guias)
        {
            guia.Cancelar();
            await _guiaRepo.UpdateAsync(guia, context.CancellationToken);
        }

        _logger.LogWarning("{Count} guias invalidadas para empresa {EmpresaId} período {Periodo}",
            guias.Count, evt.EmpresaId, evt.Periodo);
    }
}
