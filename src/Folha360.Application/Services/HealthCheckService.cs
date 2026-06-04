using Folha360.Application.DTOs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Folha360.Application.Services;

public class AppHealthCheckService : IHealthCheckService
{
    private readonly HealthCheckService _healthCheckService;

    public AppHealthCheckService(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    public async Task<HealthResponse> GetHealthAsync(CancellationToken ct = default)
    {
        var report = await _healthCheckService.CheckHealthAsync(ct);
        var services = new Dictionary<string, HealthItem>();

        foreach (var entry in report.Entries)
        {
            services[entry.Key] = new HealthItem(
                entry.Value.Status.ToString(),
                entry.Value.Description,
                entry.Value.Duration);
        }

        var overallStatus = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";
        return new HealthResponse(overallStatus, services);
    }
}
