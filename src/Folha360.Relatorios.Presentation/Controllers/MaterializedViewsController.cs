using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios/materialized-views")]
[Authorize(Policy = "Admin")]
public class MaterializedViewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MaterializedViewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshViewsCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(new { mensagem = "Materialized views atualizadas com sucesso." });
    }
}
