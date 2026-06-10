using Folha360.Fiscais.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Fiscais.Presentation.Controllers;

[ApiController]
[Route("api/fiscais/apuracao")]
[Authorize]
public class ApuracaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApuracaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{empresaId}/{periodo}")]
    [Authorize(Roles = "Contador,Operador,Admin")]
    public async Task<IActionResult> ObterApuracao(Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterApuracaoQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{empresaId}/{periodo}/detalhado")]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> ObterApuracaoDetalhada(Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterApuracaoDetalhadaQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("status/{empresaId}")]
    [Authorize(Roles = "Contador,Operador,Admin")]
    public async Task<IActionResult> ObterStatusFiscal(Guid empresaId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterStatusFiscalQuery(empresaId), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("calendario/{empresaId}/{ano}")]
    [Authorize(Roles = "Contador,Operador,Admin")]
    public async Task<IActionResult> ObterCalendarioFiscal(Guid empresaId, int ano, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterCalendarioFiscalQuery(empresaId, ano), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
