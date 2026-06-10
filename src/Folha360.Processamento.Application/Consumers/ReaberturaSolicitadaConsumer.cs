using Folha360.Processamento.Application.Hubs;
using Folha360.Processamento.Domain.Events;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Consumers;

public class ReaberturaSolicitadaConsumer : IConsumer<FolhaReabertaEvent>
{
    private readonly IHubContext<ProcessamentoHub> _hubContext;
    private readonly ILogger<ReaberturaSolicitadaConsumer> _logger;

    public ReaberturaSolicitadaConsumer(
        IHubContext<ProcessamentoHub> hubContext,
        ILogger<ReaberturaSolicitadaConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaReabertaEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "Reabertura recebida: Processamento {ProcessamentoId}, Versão {Versao}",
            msg.ProcessamentoId, msg.Versao);

        await _hubContext.Clients.Group($"empresa_{msg.EmpresaId}")
            .SendAsync("ProcessingReopened", msg.ProcessamentoId, msg.Versao);
    }
}
