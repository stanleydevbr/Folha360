using Folha360.Fiscais.Domain.Events;
using Folha360.Relatorios.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Consumers;

public class ObrigacoesApuradasConsumer : IConsumer<ObrigacoesApuradasEvent>
{
    private readonly IMediator _mediator;
    private readonly IRedisCacheService _cacheService;
    private readonly ILogger<ObrigacoesApuradasConsumer> _logger;

    public ObrigacoesApuradasConsumer(
        IMediator mediator,
        IRedisCacheService cacheService,
        ILogger<ObrigacoesApuradasConsumer> logger)
    {
        _mediator = mediator;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ObrigacoesApuradasEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Consumindo ObrigacoesApuradas: Empresa={EmpresaId}, Periodo={Periodo}", message.EmpresaId, message.Periodo);

        // Invalidate cache for this period
        await _cacheService.InvalidarAsync($"relatorios:resumo:{message.EmpresaId}:{message.Periodo}", context.CancellationToken);

        // Refresh DIRF and RAIS views
        var refreshCommand = new RefreshViewsCommand
        {
            EmpresaId = message.EmpresaId,
            Periodo = message.Periodo,
        };
        await _mediator.Send(refreshCommand, context.CancellationToken);

        _logger.LogInformation("Cache e views atualizados após ObrigacoesApuradas");
    }
}
