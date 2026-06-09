using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Fiscais.Presentation.Controllers;

[ApiController]
[Route("api/fiscais/regras")]
[Authorize]
public class RegraFiscalController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegraFiscalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> ListarRegras([FromQuery] string? tributo = null, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListarRegrasFiscaisQuery(tributo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarRegra([FromBody] CriarRegraFiscalCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? CreatedAtAction(nameof(ListarRegras), null, result) : BadRequest(result);
    }
}
