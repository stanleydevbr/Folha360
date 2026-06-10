using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios/agendamentos")]
[Authorize(Policy = "Operador")]
public class AgendamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgendamentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAgendamentoCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Created($"/api/relatorios/agendamentos/{result.Value}", new { id = result.Value });
    }

    [HttpGet("{empresaId:guid}")]
    public async Task<IActionResult> Listar(Guid empresaId)
    {
        var query = new ListarAgendamentosQuery { EmpresaId = empresaId };
        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarAgendamentoCommand command)
    {
        command.AgendamentoId = id;
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var command = new CancelarAgendamentoCommand { AgendamentoId = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/historico")]
    public async Task<IActionResult> Historico(Guid id)
    {
        var query = new ObterHistoricoAgendamentoQuery { AgendamentoId = id };
        var result = await _mediator.Send(query);
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/executar")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Executar(Guid id)
    {
        var service = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IAgendamentoService>();
        await service.ExecutarAsync(id, HttpContext.RequestAborted);
        return Accepted();
    }
}
