using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Eventos.Application.Services;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Eventos.Application.Consumers;

public class GerarXmlAdmissaoConsumer : IConsumer<GerarXmlAdmissaoCommand>
{
    private readonly IAdmissaoRepository _repo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IXmlGeradorService _xmlService;
    private readonly ILogger<GerarXmlAdmissaoConsumer> _logger;

    public GerarXmlAdmissaoConsumer(
        IAdmissaoRepository repo,
        IFuncionarioRepository funcionarioRepo,
        IEmpresaRepository empresaRepo,
        IXmlGeradorService xmlService,
        ILogger<GerarXmlAdmissaoConsumer> logger)
    {
        _repo = repo;
        _funcionarioRepo = funcionarioRepo;
        _empresaRepo = empresaRepo;
        _xmlService = xmlService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GerarXmlAdmissaoCommand> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Gerando XML S-2200 para AdmissaoId={AdmissaoId}", msg.AdmissaoId);

        var admissao = await _repo.GetByIdAsync(msg.AdmissaoId, context.CancellationToken);
        if (admissao is null)
        {
            _logger.LogWarning("Admissao {AdmissaoId} não encontrada para geração de XML", msg.AdmissaoId);
            return;
        }

        var funcionario = await _funcionarioRepo.GetByIdAsync(admissao.FuncionarioId, context.CancellationToken);
        var empresa = await _empresaRepo.GetByIdAsync(admissao.EmpresaId, context.CancellationToken);

        if (funcionario is null || empresa is null)
        {
            _logger.LogWarning("Dados complementares não encontrados para AdmissaoId={AdmissaoId}", msg.AdmissaoId);
            return;
        }

        var xml = _xmlService.GerarXmlAdmissao(admissao, empresa, funcionario);
        var validacao = _xmlService.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2200.xsd");

        if (!validacao.IsValid)
        {
            _logger.LogWarning("XML S-2200 inválido para AdmissaoId={AdmissaoId}: {Errors}",
                msg.AdmissaoId, string.Join("; ", validacao.Errors));
        }

        admissao.XmlContent = xml;
        await _repo.UpdateAsync(admissao, context.CancellationToken);

        _logger.LogInformation("XML S-2200 gerado com sucesso para AdmissaoId={AdmissaoId}", msg.AdmissaoId);
    }
}
