using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Processamento.Presentation.Controllers;

[ApiController]
[Route("api/folha/holerites")]
[Authorize(Policy = "Consulta")]
public class HoleriteController : ControllerBase
{
    private readonly IMediator _mediator;

    public HoleriteController(IMediator mediator) => _mediator = mediator;

    /// <summary>Listar holerites do processamento (RF40)</summary>
    [HttpGet("{processamentoId:guid}")]
    [ProducesResponseType(typeof(List<HoleriteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HoleriteResponse>>> Listar(
        Guid processamentoId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarHoleritesQuery(processamentoId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    /// <summary>Download holerite PDF (RF41)</summary>
    [HttpGet("{processamentoId:guid}/{funcionarioId:guid}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(
        Guid processamentoId, Guid funcionarioId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ObterHoleriteQuery(processamentoId, funcionarioId), ct);

        if (!result.IsSuccess)
            return NotFound(new { result.Errors });

        return File(result.Value!, "application/pdf", $"holerite_{funcionarioId}.pdf");
    }
}
