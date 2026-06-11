using Folha360.Esocial.Application.Commands;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Abstractions;
using Folha360.Esocial.Domain.Entities;
using Folha360.Eventos.Domain.Events;
using Folha360.Processamento.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Esocial.Application.Consumers;

public class EventoRemuneracaoGeradoConsumer : IConsumer<EventoRemuneracaoGeradoEvent>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<EventoRemuneracaoGeradoConsumer> _logger;

    public EventoRemuneracaoGeradoConsumer(
        IEventoEsocialRepository eventoRepo,
        ILogger<EventoRemuneracaoGeradoConsumer> logger)
    {
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EventoRemuneracaoGeradoEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Recebido evento de remuneração: Empresa={EmpresaId}, Funcionario={FuncionarioId}",
            msg.EmpresaId, msg.FuncionarioId);

        var idEvento = $"ID{msg.EmpresaId.ToString("N")[..8]}{msg.FuncionarioId.ToString("N")[..8]}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var evento = new EventoEsocial(
            msg.EmpresaId,
            TipoEventoEsocial.S1200,
            $"<eSocial xmlns=\"http://www.esocial.gov.br/schema/lote/eventos/envio/v1_1_1\"><!-- S-1200 --></eSocial>",
            idEvento,
            msg.FuncionarioId);

        await _eventoRepo.AdicionarAsync(evento, context.CancellationToken);
        _logger.LogInformation("Evento S-1200 criado: {IdEvento}", idEvento);
    }
}

public class FolhaFechadaEsocialConsumer : IConsumer<FolhaFechadaEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<FolhaFechadaEsocialConsumer> _logger;

    public FolhaFechadaEsocialConsumer(IMediator mediator, ILogger<FolhaFechadaEsocialConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FolhaFechadaEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Folha fechada: Empresa={EmpresaId}, Periodo={Periodo}", msg.EmpresaId, msg.Periodo);

        var command = new EnviarEventosEsocialCommand(msg.EmpresaId, msg.Periodo);
        await _mediator.Send(command, context.CancellationToken);
    }
}

public class FuncionarioAdmitidoEsocialConsumer : IConsumer<FuncionarioAdmitidoEvent>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<FuncionarioAdmitidoEsocialConsumer> _logger;

    public FuncionarioAdmitidoEsocialConsumer(
        IEventoEsocialRepository eventoRepo,
        ILogger<FuncionarioAdmitidoEsocialConsumer> logger)
    {
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FuncionarioAdmitidoEvent> context)
    {
        var msg = context.Message;
        var idEvento = $"ID{msg.EmpresaId.ToString("N")[..8]}{msg.FuncionarioId.ToString("N")[..8]}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var evento = new EventoEsocial(
            msg.EmpresaId,
            TipoEventoEsocial.S2200,
            $"<eSocial xmlns=\"http://www.esocial.gov.br/schema/lote/eventos/envio/v1_1_1\"><!-- S-2200 --></eSocial>",
            idEvento,
            msg.FuncionarioId);

        await _eventoRepo.AdicionarAsync(evento, context.CancellationToken);
        _logger.LogInformation("Evento S-2200 criado: {IdEvento}", idEvento);
    }
}

public class FuncionarioDesligadoEsocialConsumer : IConsumer<FuncionarioDesligadoEvent>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<FuncionarioDesligadoEsocialConsumer> _logger;

    public FuncionarioDesligadoEsocialConsumer(
        IEventoEsocialRepository eventoRepo,
        ILogger<FuncionarioDesligadoEsocialConsumer> logger)
    {
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FuncionarioDesligadoEvent> context)
    {
        var msg = context.Message;
        var idEvento = $"ID{msg.EmpresaId.ToString("N")[..8]}{msg.FuncionarioId.ToString("N")[..8]}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var evento = new EventoEsocial(
            msg.EmpresaId,
            TipoEventoEsocial.S2299,
            $"<eSocial xmlns=\"http://www.esocial.gov.br/schema/lote/eventos/envio/v1_1_1\"><!-- S-2299 --></eSocial>",
            idEvento,
            msg.FuncionarioId);

        await _eventoRepo.AdicionarAsync(evento, context.CancellationToken);
        _logger.LogInformation("Evento S-2299 criado: {IdEvento}", idEvento);
    }
}

public class FeriasConcedidasEsocialConsumer : IConsumer<FeriasConcedidasEvent>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<FeriasConcedidasEsocialConsumer> _logger;

    public FeriasConcedidasEsocialConsumer(
        IEventoEsocialRepository eventoRepo,
        ILogger<FeriasConcedidasEsocialConsumer> logger)
    {
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FeriasConcedidasEvent> context)
    {
        var msg = context.Message;
        var idEvento = $"ID{msg.EmpresaId.ToString("N")[..8]}{msg.FuncionarioId.ToString("N")[..8]}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var evento = new EventoEsocial(
            msg.EmpresaId,
            TipoEventoEsocial.S2230,
            $"<eSocial xmlns=\"http://www.esocial.gov.br/schema/lote/eventos/envio/v1_1_1\"><!-- S-2230 --></eSocial>",
            idEvento,
            msg.FuncionarioId);

        await _eventoRepo.AdicionarAsync(evento, context.CancellationToken);
        _logger.LogInformation("Evento S-2230 criado: {IdEvento}", idEvento);
    }
}
