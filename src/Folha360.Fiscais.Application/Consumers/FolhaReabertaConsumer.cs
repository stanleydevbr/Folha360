using Folha360.Processamento.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Consumers;

public class FolhaReabertaConsumer : IConsumer<FolhaReabertaEvent>
{
    private readonly ILogger<FolhaReabertaConsumer> _logger;

    public FolhaReabertaConsumer(ILogger<FolhaReabertaConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<FolhaReabertaEvent> context)
    {
        var evt = context.Message;
        _logger.LogWarning("Folha reaberta para empresa {EmpresaId} período {Periodo}. Guias fiscais devem ser revisadas.",
            evt.EmpresaId, evt.Periodo);

        return Task.CompletedTask;
    }
}
