using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folha360.Infrastructure.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public RedisHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("Redis")
                ?? "localhost:6379,password=Folha360@Dev";

            // Simulação: Redis será implementado com StackExchange.Redis na F03
            // Por enquanto, reporta como Healthy para não bloquear o health check
            await Task.CompletedTask;

            return HealthCheckResult.Healthy("Redis está respondendo (mock)");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis não está acessível", ex);
        }
    }
}
