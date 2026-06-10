using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios/holerites")]
[Authorize(Policy = "Operador")]
public class HoleritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public HoleritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("lote")]
    public async Task<IActionResult> GerarLote([FromBody] GerarHoleritesLoteCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        var loteId = result.Value;
        return Accepted(new { lote_id = loteId, status_url = $"/api/relatorios/holerites/lote/{loteId}/status" });
    }

    [HttpGet("lote/{loteId:guid}/status")]
    public IActionResult ObterStatusLote(Guid loteId)
    {
        // Simplified status — in production, track via distributed cache or DB
        return Ok(new LoteStatusDto
        {
            LoteId = loteId,
            Status = "concluido",
            ProgressoPercentual = 100,
            EstimativaSegundos = 0,
        });
    }

    [HttpGet("{empresaId:guid}/{periodo}")]
    public IActionResult ListarHolerites(Guid empresaId, string periodo)
    {
        // Simplified — in production, list from relatorio_arquivo table
        return Ok(new { empresa_id = empresaId, periodo, holerites = Array.Empty<object>() });
    }
}
