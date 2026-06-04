using Folha360.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;

    public HealthController(IHealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Application.DTOs.HealthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealth(CancellationToken ct)
    {
        var health = await _healthCheckService.GetHealthAsync(ct);
        return Ok(health);
    }

    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetReady()
    {
        return Ok(new { status = "Ready" });
    }

    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLive()
    {
        return Ok(new { status = "Alive" });
    }
}
