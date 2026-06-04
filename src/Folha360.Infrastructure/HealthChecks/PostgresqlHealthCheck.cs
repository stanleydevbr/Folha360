using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Folha360.Infrastructure.HealthChecks;

public class PostgresqlHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public PostgresqlHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres")
            ?? "Host=localhost;Port=5432;Database=folha360;Username=folha360_user;Password=Folha360@Dev";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 3;

            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("PostgreSQL está respondendo");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "PostgreSQL não está acessível",
                ex);
        }
    }
}
