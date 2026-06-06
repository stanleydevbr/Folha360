using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Events;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Domain.Events;
using Folha360.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class FuncionarioCadastradoConsumer : IConsumer<FuncionarioCadastradoEvent>
{
    private readonly IAdmissaoRepository _admissaoRepo;
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<FuncionarioCadastradoConsumer> _logger;

    public FuncionarioCadastradoConsumer(
        IAdmissaoRepository admissaoRepo,
        IMessageBus messageBus,
        IPublishEndpoint publishEndpoint,
        ILogger<FuncionarioCadastradoConsumer> logger)
    {
        _admissaoRepo = admissaoRepo;
        _messageBus = messageBus;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FuncionarioCadastradoEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "Consumindo FuncionarioCadastrado: FuncionarioId={FuncionarioId}, CorrelationId={CorrelationId}",
            msg.FuncionarioId, context.CorrelationId);

        var admissao = new Admissao(
            msg.FuncionarioId,
            msg.EmpresaId,
            msg.DataAdmissao,
            msg.CargoId,
            msg.SalarioBase,
            Domain.TipoContrato.Indeterminado);

        await _admissaoRepo.AddAsync(admissao, context.CancellationToken);

        await _messageBus.PublishAsync(
            new FuncionarioAdmitidoEvent(admissao.FuncionarioId, admissao.EmpresaId, admissao.Id),
            "folha360.eventos", "FuncionarioAdmitido", context.CancellationToken);

        await _publishEndpoint.Publish(
            new GerarXmlAdmissaoCommand(admissao.Id), context.CancellationToken);

        _logger.LogInformation(
            "Admissao criada automaticamente: AdmissaoId={AdmissaoId} para FuncionarioId={FuncionarioId}",
            admissao.Id, msg.FuncionarioId);
    }
}
