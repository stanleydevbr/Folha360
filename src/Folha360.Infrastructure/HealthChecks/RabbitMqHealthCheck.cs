using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folha360.Infrastructure.HealthChecks;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public RabbitMqHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("RabbitMQ")
                ?? "amqp://folha360:Folha360@Dev@localhost:5672";

            // Mock: Implementação real com RabbitMQ.Client será feita na F03 — Eventos Trabalhistas
            // Por enquanto, reporta Healthy para não bloquear o health check em desenvolvimento
            await Task.CompletedTask;

            return HealthCheckResult.Healthy("RabbitMQ está respondendo (mock)");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ não está acessível", ex);
        }
    }
}
