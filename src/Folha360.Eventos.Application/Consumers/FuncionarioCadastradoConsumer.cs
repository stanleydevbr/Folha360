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
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<FuncionarioCadastradoConsumer> _logger;

    public FuncionarioCadastradoConsumer(
        IAdmissaoRepository admissaoRepo,
        IFuncionarioRepository funcionarioRepo,
        IMessageBus messageBus,
        IPublishEndpoint publishEndpoint,
        ILogger<FuncionarioCadastradoConsumer> logger)
    {
        _admissaoRepo = admissaoRepo;
        _funcionarioRepo = funcionarioRepo;
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

        // Verifica se já existe admissão ativa para este funcionário
        var admissoesExistentes = await _admissaoRepo.GetPagedAsync(1, 1, msg.FuncionarioId, context.CancellationToken);
        if (admissoesExistentes.TotalCount > 0)
        {
            _logger.LogWarning(
                "Admissao já existe para FuncionarioId={FuncionarioId}. Ignorando evento duplicado.",
                msg.FuncionarioId);
            return;
        }

        // Busca dados complementares do funcionário
        var funcionario = await _funcionarioRepo.GetByIdAsync(msg.FuncionarioId, context.CancellationToken);
        if (funcionario is null)
        {
            _logger.LogWarning(
                "Funcionario {FuncionarioId} não encontrado. Ignorando evento.",
                msg.FuncionarioId);
            return;
        }

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
