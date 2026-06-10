using Folha360.Processamento.Domain.Events;
using Folha360.Relatorios.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Consumers;

public class FolhaFechadaConsumer : IConsumer<FolhaFechadaEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<FolhaFechadaConsumer> _logger;

    public FolhaFechadaConsumer(IMediator mediator, ILogger<FolhaFechadaConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaFechadaEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Consumindo FolhaFechada: Empresa={EmpresaId}, Periodo={Periodo}", message.EmpresaId, message.Periodo);

        // (a) Refresh materialized views
        var refreshCommand = new RefreshViewsCommand
        {
            EmpresaId = message.EmpresaId,
            Periodo = message.Periodo,
        };
        await _mediator.Send(refreshCommand, context.CancellationToken);

        // (b) Publish RelatoriosAtualizadosEvent
        var atualizadosEvent = new Folha360.Relatorios.Domain.Events.RelatoriosAtualizadosEvent
        {
            EmpresaId = message.EmpresaId,
            Periodo = message.Periodo,
            TiposRelatorio = new List<string> { "folha_analitica", "folha_sintetica", "resumo_mensal" },
            CorrelationId = context.CorrelationId ?? Guid.NewGuid(),
            CausationId = context.MessageId ?? Guid.NewGuid(),
        };

        await context.Publish(atualizadosEvent, context.CancellationToken);
        _logger.LogInformation("RelatoriosAtualizados publicado para Empresa={EmpresaId}, Periodo={Periodo}", message.EmpresaId, message.Periodo);
    }
}
