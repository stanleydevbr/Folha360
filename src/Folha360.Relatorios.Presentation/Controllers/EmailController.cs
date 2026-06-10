using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Policy = "Operador")]
public class EmailController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("enviar-email")]
    public async Task<IActionResult> EnviarEmail([FromBody] EnviarEmailCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(new { mensagem = "Email enviado com sucesso." });
    }
}
