using Folha360.Fiscais.Application.Commands;
using Folha360.Processamento.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Consumers;

public class ApurarObrigacoesFiscaisConsumer : IConsumer<FolhaFechadaEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApurarObrigacoesFiscaisConsumer> _logger;

    public ApurarObrigacoesFiscaisConsumer(IMediator mediator, ILogger<ApurarObrigacoesFiscaisConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaFechadaEvent> context)
    {
        var evt = context.Message;
        _logger.LogInformation("Iniciando apuração fiscal para empresa {EmpresaId} período {Periodo}",
            evt.EmpresaId, evt.Periodo);

        var command = new ApurarObrigacoesCommand(evt.EmpresaId, evt.Periodo, evt.ProcessamentoId);
        await _mediator.Send(command, context.CancellationToken);
    }
}
