using Folha360.Esocial.Domain.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folha360.Esocial.Infrastructure.Services;

public class EsocialHealthCheck : IHealthCheck
{
    private readonly ICertificadoDigitalRepository _certificadoRepo;

    public EsocialHealthCheck(ICertificadoDigitalRepository certificadoRepo)
    {
        _certificadoRepo = certificadoRepo;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        var data = new Dictionary<string, object>();
        var warnings = new List<string>();

        try
        {
            // Verificar certificados próximos da expiração
            var certificados = await _certificadoRepo.ObterProximosExpiracaoAsync(30, ct);
            var expirados = certificados.Where(c => c.EstaExpirado).ToList();
            var expirando = certificados.Where(c => !c.EstaExpirado && c.EstaExpirandoEmBreve(30)).ToList();

            data["certificados_expirados"] = expirados.Count;
            data["certificados_expirando"] = expirando.Count;

            if (expirados.Any())
            {
                warnings.Add($"{expirados.Count} certificado(s) expirado(s)");
            }

            if (expirando.Any())
            {
                warnings.Add($"{expirando.Count} certificado(s) expirando em menos de 30 dias");
            }

            if (warnings.Any())
            {
                return HealthCheckResult.Degraded(string.Join("; ", warnings), data: data);
            }

            data["status"] = "OK";
            return HealthCheckResult.Healthy("Integração e-Social operacional.", data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Erro ao verificar integração e-Social.", ex, data);
        }
    }
}
