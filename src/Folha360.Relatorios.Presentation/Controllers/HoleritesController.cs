using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Folha360.Relatorios.Application.Services;

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
        {
            return BadRequest(result.Errors);
        }

        var loteId = result.Value;
        return Accepted(new { lote_id = loteId, status_url = $"/api/relatorios/holerites/lote/{loteId}/status" });
    }

    [HttpGet("lote/{loteId:guid}/status")]
    public async Task<IActionResult> ObterStatusLote(Guid loteId)
    {
        var cache = HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();
        var status = await cache.ObterAsync<LoteStatusDto>($"lote:{loteId}", HttpContext.RequestAborted);

        if (status is null)
        {
            return NotFound(new { mensagem = "Lote não encontrado ou expirado." });
        }

        return Ok(status);
    }

    [HttpGet("{empresaId:guid}/{periodo}")]
    public async Task<IActionResult> ListarHolerites(Guid empresaId, string periodo)
    {
        var agendamentoRepo = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Domain.Abstractions.IAgendamentoRepository>();
        var arquivos = await agendamentoRepo.ListarArquivosAsync(empresaId, periodo, HttpContext.RequestAborted);

        var holerites = arquivos
            .Where(a => a.TipoRelatorio == Folha360.Relatorios.Domain.Enums.TipoRelatorio.Holerite)
            .Select(a => new
            {
                funcionario_id = a.Chave.Split('/').Last().Replace(".pdf", string.Empty),
                periodo = a.Periodo,
                criado_em = a.CriadoEm,
                download_url = $"/api/relatorios/holerites/{empresaId}/{periodo}/{a.Chave.Split('/').Last()}",
            })
            .ToList();

        return Ok(new { empresa_id = empresaId, periodo, holerites });
    }
}
