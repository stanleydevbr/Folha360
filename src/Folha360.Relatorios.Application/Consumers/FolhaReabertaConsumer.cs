using Folha360.Processamento.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Consumers;

public class FolhaReabertaConsumer : IConsumer<FolhaReabertaEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<FolhaReabertaConsumer> _logger;

    public FolhaReabertaConsumer(IMediator mediator, ILogger<FolhaReabertaConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaReabertaEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Consumindo FolhaReaberta: Empresa={EmpresaId}, Periodo={Periodo}", message.EmpresaId, message.Periodo);

        var command = new InvalidarRelatoriosCommand
        {
            EmpresaId = message.EmpresaId,
            Periodo = message.Periodo,
        };

        await _mediator.Send(command, context.CancellationToken);
        _logger.LogInformation("Relatórios invalidados para Empresa={EmpresaId}, Periodo={Periodo}", message.EmpresaId, message.Periodo);
    }
}
