using Folha360.Esocial.Domain.Abstractions;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Folha360.Esocial.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class ConsultarReciboJob : IJob
{
    private readonly ILoteEsocialRepository _loteRepo;
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly IEsocialEnvioService _envioService;
    private readonly ILogger<ConsultarReciboJob> _logger;

    public ConsultarReciboJob(
        ILoteEsocialRepository loteRepo,
        IEventoEsocialRepository eventoRepo,
        IEsocialEnvioService envioService,
        ILogger<ConsultarReciboJob> logger)
    {
        _loteRepo = loteRepo;
        _eventoRepo = eventoRepo;
        _envioService = envioService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("ConsultarReciboJob iniciado.");

        try
        {
            var lotesParaConsultar = await _loteRepo.ObterLotesEnviadosPendentesAsync(context.CancellationToken);

            foreach (var lote in lotesParaConsultar)
            {
                if (string.IsNullOrWhiteSpace(lote.ProtocoloEnvio))
                    continue;

                try
                {
                    var resposta = await _envioService.ConsultarReciboAsync(
                        lote.ProtocoloEnvio,
                        lote.TipoAmbiente,
                        context.CancellationToken);

                    lote.Processar(resposta);
                    await _loteRepo.AtualizarAsync(lote, context.CancellationToken);

                    _logger.LogInformation("Lote {LoteId} processado com sucesso. Protocolo: {Protocolo}",
                        lote.Id, lote.ProtocoloEnvio);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consultar recibo do lote {LoteId}", lote.Id);
                }
            }

            _logger.LogInformation("ConsultarReciboJob concluído. {Count} lotes consultados.", lotesParaConsultar.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no ConsultarReciboJob.");
        }
    }
}

[DisallowConcurrentExecution]
public class AtualizarSchemaJob : IJob
{
    private readonly IXsdSchemaService _schemaService;
    private readonly ILogger<AtualizarSchemaJob> _logger;

    public AtualizarSchemaJob(IXsdSchemaService schemaService, ILogger<AtualizarSchemaJob> logger)
    {
        _schemaService = schemaService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("AtualizarSchemaJob iniciado.");
        await _schemaService.AtualizarSchemasAsync(context.CancellationToken);
    }
}

[DisallowConcurrentExecution]
public class VerificarCertificadoExpiracaoJob : IJob
{
    private readonly ICertificadoDigitalRepository _certificadoRepo;
    private readonly ILogger<VerificarCertificadoExpiracaoJob> _logger;

    public VerificarCertificadoExpiracaoJob(
        ICertificadoDigitalRepository certificadoRepo,
        ILogger<VerificarCertificadoExpiracaoJob> logger)
    {
        _certificadoRepo = certificadoRepo;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("VerificarCertificadoExpiracaoJob iniciado.");

        try
        {
            var certificados = await _certificadoRepo.ObterProximosExpiracaoAsync(30, context.CancellationToken);
            foreach (var cert in certificados)
            {
                if (cert.EstaExpirado)
                {
                    _logger.LogWarning("Certificado {Id} EXPIRADO em {Data}", cert.Id, cert.DataExpiracao);
                }
                else
                {
                    _logger.LogInformation("Certificado {Id} expira em {Dias} dias ({Data})",
                        cert.Id, cert.DiasRestantes, cert.DataExpiracao);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no VerificarCertificadoExpiracaoJob.");
        }
    }
}
