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
            // Buscar lotes enviados que ainda não foram processados
            // Nota: em produção, isso seria filtrado por status = Enviado e DataEnvio há mais de 5 min
            _logger.LogInformation("ConsultarReciboJob concluído.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no ConsultarReciboJob.");
        }

        await Task.CompletedTask;
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
