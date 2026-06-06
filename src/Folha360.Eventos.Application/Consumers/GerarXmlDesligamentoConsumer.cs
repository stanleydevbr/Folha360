using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class GerarXmlDesligamentoConsumer : IConsumer<GerarXmlDesligamentoCommand>
{
    private readonly IDesligamentoRepository _repo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IXmlGeradorService _xmlService;
    private readonly ILogger<GerarXmlDesligamentoConsumer> _logger;

    public GerarXmlDesligamentoConsumer(
        IDesligamentoRepository repo,
        IFuncionarioRepository funcionarioRepo,
        IEmpresaRepository empresaRepo,
        IXmlGeradorService xmlService,
        ILogger<GerarXmlDesligamentoConsumer> logger)
    {
        _repo = repo;
        _funcionarioRepo = funcionarioRepo;
        _empresaRepo = empresaRepo;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GerarXmlDesligamentoCommand> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Gerando XML S-2299 para DesligamentoId={DesligamentoId}", msg.DesligamentoId);

        var desligamento = await _repo.GetByIdAsync(msg.DesligamentoId, context.CancellationToken);
        if (desligamento is null)
        {
            return;
        }

        var funcionario = await _funcionarioRepo.GetByIdAsync(desligamento.FuncionarioId, context.CancellationToken);
        var empresa = await _empresaRepo.GetByIdAsync(desligamento.EmpresaId, context.CancellationToken);
        if (funcionario is null || empresa is null)
        {
            return;
        }

        var xml = _xmlService.GerarXmlDesligamento(desligamento, empresa, funcionario);
        var validacao = _xmlService.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2299.xsd");

        if (!validacao.IsValid)
        {
            _logger.LogWarning(
                "XML S-2299 inválido para DesligamentoId={DesligamentoId}: {Errors}",
                msg.DesligamentoId,
                string.Join("; ", validacao.Errors));
        }

        desligamento.XmlContent = xml;
        await _repo.UpdateAsync(desligamento, context.CancellationToken);
    }
}
