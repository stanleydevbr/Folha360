using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Folha360.Relatorios.Infrastructure.Health;

public class ReadReplicaHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReadReplicaHealthCheck> _logger;

    public ReadReplicaHealthCheck(
        IConfiguration configuration,
        ILogger<ReadReplicaHealthCheck> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        var connectionString = _configuration.GetConnectionString("PostgresReadReplica");

        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string do read replica não configurada. Usando fallback para primário.");
            return HealthCheckResult.Degraded("Read replica não configurado — usando primário como fallback.");
        }

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(ct);

            // Check replication lag
            await using var cmd = new NpgsqlCommand(
                "SELECT EXTRACT(EPOCH FROM (NOW() - pg_last_xact_replay_timestamp())) AS lag_seconds",
                connection);

            var result = await cmd.ExecuteScalarAsync(ct);

            if (result is not DBNull && result is not null)
            {
                var lagSeconds = Convert.ToDouble(result);

                // 5 minutes threshold
                if (lagSeconds > 300)
                {
                    _logger.LogWarning("Read replica lag elevado: {LagSeconds}s", lagSeconds);
                    return HealthCheckResult.Degraded($"Read replica lag: {lagSeconds:F0}s (threshold: 300s)");
                }
            }

            return HealthCheckResult.Healthy("Read replica conectado e sincronizado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha no health check do read replica");
            return HealthCheckResult.Unhealthy("Read replica indisponível.", ex);
        }
    }
}
