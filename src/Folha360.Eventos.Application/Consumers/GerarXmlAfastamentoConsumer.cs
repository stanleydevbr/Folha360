using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class GerarXmlAfastamentoConsumer : IConsumer<GerarXmlAfastamentoCommand>
{
    private readonly IAfastamentoRepository _repo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IXmlGeradorService _xmlService;
    private readonly ILogger<GerarXmlAfastamentoConsumer> _logger;

    public GerarXmlAfastamentoConsumer(
        IAfastamentoRepository repo,
        IFuncionarioRepository funcionarioRepo,
        IEmpresaRepository empresaRepo,
        IXmlGeradorService xmlService,
        ILogger<GerarXmlAfastamentoConsumer> logger)
    {
        _repo = repo;
        _funcionarioRepo = funcionarioRepo;
        _empresaRepo = empresaRepo;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GerarXmlAfastamentoCommand> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Gerando XML S-2231 para AfastamentoId={AfastamentoId}", msg.AfastamentoId);

        var afastamento = await _repo.GetByIdAsync(msg.AfastamentoId, context.CancellationToken);
        if (afastamento is null)
        {
            _logger.LogWarning("Afastamento {AfastamentoId} não encontrado para geração de XML", msg.AfastamentoId);
            return;
        }

        var funcionario = await _funcionarioRepo.GetByIdAsync(afastamento.FuncionarioId, context.CancellationToken);
        var empresa = await _empresaRepo.GetByIdAsync(afastamento.EmpresaId, context.CancellationToken);
        if (funcionario is null || empresa is null)
        {
            _logger.LogWarning("Dados complementares não encontrados para AfastamentoId={AfastamentoId}", msg.AfastamentoId);
            return;
        }

        var xml = _xmlService.GerarXmlAfastamento(afastamento, empresa, funcionario);
        var validacao = _xmlService.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2231.xsd");

        if (!validacao.IsValid)
        {
            _logger.LogWarning(
                "XML S-2231 inválido para AfastamentoId={AfastamentoId}: {Errors}",
                msg.AfastamentoId,
                string.Join("; ", validacao.Errors));
        }

        afastamento.XmlContent = xml;
        await _repo.UpdateAsync(afastamento, context.CancellationToken);
    }
}
