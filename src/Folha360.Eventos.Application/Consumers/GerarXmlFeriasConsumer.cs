using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class GerarXmlFeriasConsumer : IConsumer<GerarXmlFeriasCommand>
{
    private readonly IFeriasRepository _repo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IXmlGeradorService _xmlService;
    private readonly ILogger<GerarXmlFeriasConsumer> _logger;

    public GerarXmlFeriasConsumer(
        IFeriasRepository repo,
        IFuncionarioRepository funcionarioRepo,
        IEmpresaRepository empresaRepo,
        IXmlGeradorService xmlService,
        ILogger<GerarXmlFeriasConsumer> logger)
    {
        _repo = repo;
        _funcionarioRepo = funcionarioRepo;
        _empresaRepo = empresaRepo;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GerarXmlFeriasCommand> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Gerando XML S-2230 para FeriasId={FeriasId}", msg.FeriasId);

        var ferias = await _repo.GetByIdAsync(msg.FeriasId, context.CancellationToken);
        if (ferias is null)
        {
            _logger.LogWarning("Ferias {FeriasId} não encontrada para geração de XML", msg.FeriasId);
            return;
        }

        var funcionario = await _funcionarioRepo.GetByIdAsync(ferias.FuncionarioId, context.CancellationToken);
        var empresa = await _empresaRepo.GetByIdAsync(ferias.EmpresaId, context.CancellationToken);
        if (funcionario is null || empresa is null)
        {
            _logger.LogWarning("Dados complementares não encontrados para FeriasId={FeriasId}", msg.FeriasId);
            return;
        }

        var xml = _xmlService.GerarXmlFerias(ferias, empresa, funcionario);
        var validacao = _xmlService.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2230.xsd");

        if (!validacao.IsValid)
        {
            _logger.LogWarning(
                "XML S-2230 inválido para FeriasId={FeriasId}: {Errors}",
                msg.FeriasId,
                string.Join("; ", validacao.Errors));
        }

        ferias.XmlContent = xml;
        await _repo.UpdateAsync(ferias, context.CancellationToken);
    }
}
