using Folha360.Cadastros.Application;
using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Eventos.Presentation.Controllers;

[ApiController]
[Route("api/ferias")]
[Authorize(Policy = "Operador")]
public class FeriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeriasController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<FeriasDto>> Criar([FromBody] CriarFeriasCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value)
            : UnprocessableEntity(result.Errors);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<FeriasDto>> Obter(Guid id)
    {
        var result = await _mediator.Send(new ObterFeriasQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<FeriasDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? funcionarioId = null)
    {
        var result = await _mediator.Send(new ListarFeriasQuery(page, pageSize, funcionarioId));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FeriasDto>> Atualizar(Guid id, [FromBody] AtualizarFeriasCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var result = await _mediator.Send(new ExcluirFeriasCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors);
    }
}
