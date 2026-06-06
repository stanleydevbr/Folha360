using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class GerarXmlAlteracaoContratualConsumer : IConsumer<GerarXmlAlteracaoContratualCommand>
{
    private readonly IAlteracaoContratualRepository _repo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IXmlGeradorService _xmlService;
    private readonly ILogger<GerarXmlAlteracaoContratualConsumer> _logger;

    public GerarXmlAlteracaoContratualConsumer(
        IAlteracaoContratualRepository repo,
        IFuncionarioRepository funcionarioRepo,
        IEmpresaRepository empresaRepo,
        IXmlGeradorService xmlService,
        ILogger<GerarXmlAlteracaoContratualConsumer> logger)
    {
        _repo = repo;
        _funcionarioRepo = funcionarioRepo;
        _empresaRepo = empresaRepo;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GerarXmlAlteracaoContratualCommand> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Gerando XML S-2206 para AlteracaoContratualId={AlteracaoContratualId}", msg.AlteracaoContratualId);

        var alteracao = await _repo.GetByIdAsync(msg.AlteracaoContratualId, context.CancellationToken);
        if (alteracao is null)
        {
            return;
        }

        var funcionario = await _funcionarioRepo.GetByIdAsync(alteracao.FuncionarioId, context.CancellationToken);
        var empresa = await _empresaRepo.GetByIdAsync(alteracao.EmpresaId, context.CancellationToken);
        if (funcionario is null || empresa is null)
        {
            return;
        }

        var xml = _xmlService.GerarXmlAlteracaoContratual(alteracao, empresa, funcionario);
        var validacao = _xmlService.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2206.xsd");

        if (!validacao.IsValid)
        {
            _logger.LogWarning(
                "XML S-2206 inválido para AlteracaoContratualId={AlteracaoContratualId}: {Errors}",
                msg.AlteracaoContratualId,
                string.Join("; ", validacao.Errors));
        }

        alteracao.XmlContent = xml;
        await _repo.UpdateAsync(alteracao, context.CancellationToken);
    }
}
