using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Fiscais.Presentation.Controllers;

[ApiController]
[Route("api/fiscais/guias")]
[Authorize]
public class GuiaController : ControllerBase
{
    private readonly IMediator _mediator;

    public GuiaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{empresaId}/{periodo}")]
    [Authorize(Roles = "Contador,Operador,Admin")]
    public async Task<IActionResult> ListarGuias(Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarGuiasQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{empresaId}/{periodo}/{tributo}")]
    [Authorize(Roles = "Contador,Operador,Admin")]
    public async Task<IActionResult> ObterGuia(Guid empresaId, string periodo, string tributo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterGuiaQuery(empresaId, periodo, tributo), ct);
        if (result.IsSuccess && result.Value != null)
        {
            Response.Headers["Content-Disposition"] = $"attachment; filename=\"{tributo}_{periodo}.pdf\"";
            return File(result.Value, "application/pdf");
        }

        return BadRequest(result);
    }

    [HttpGet("{empresaId}/{periodo}/{tributo}/dados")]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> ObterDadosGuia(Guid empresaId, string periodo, string tributo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterDadosGuiaQuery(empresaId, periodo, tributo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{guiaId}/registrar-pagamento")]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> RegistrarPagamento(Guid guiaId, [FromBody] RegistrarPagamentoGuiaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
