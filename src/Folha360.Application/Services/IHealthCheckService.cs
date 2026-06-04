using Folha360.Application.DTOs;

namespace Folha360.Application.Services;

public interface IHealthCheckService
{
    Task<HealthResponse> GetHealthAsync(CancellationToken ct = default);
}
