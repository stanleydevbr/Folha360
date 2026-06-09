using Folha360.Processamento.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Consumers;

public class ReverterObrigacoesConsumer : IConsumer<Folha360.Processamento.Domain.Events.ReverterObrigacoesCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReverterObrigacoesConsumer> _logger;

    public ReverterObrigacoesConsumer(IMediator mediator, ILogger<ReverterObrigacoesConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Folha360.Processamento.Domain.Events.ReverterObrigacoesCommand> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Revertendo obrigações fiscais para empresa {EmpresaId} período {Periodo}",
            evt.EmpresaId, evt.Periodo);

        var command = new Commands.ReverterObrigacoesCommand(evt.EmpresaId, evt.Periodo);
        await _mediator.Send(command, context.CancellationToken);
    }
}
