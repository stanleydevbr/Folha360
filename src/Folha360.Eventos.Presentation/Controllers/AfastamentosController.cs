using Folha360.Cadastros.Application;
using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Eventos.Presentation.Controllers;

[ApiController]
[Route("api/afastamentos")]
[Authorize(Policy = "Operador")]
public class AfastamentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public AfastamentosController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<AfastamentoDto>> Criar([FromBody] CriarAfastamentoCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value)
            : UnprocessableEntity(result.Errors);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<AfastamentoDto>> Obter(Guid id)
    {
        var result = await _mediator.Send(new ObterAfastamentoQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<AfastamentoDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? funcionarioId = null)
    {
        var result = await _mediator.Send(new ListarAfastamentosQuery(page, pageSize, funcionarioId));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AfastamentoDto>> Atualizar(Guid id, [FromBody] AtualizarAfastamentoCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var result = await _mediator.Send(new ExcluirAfastamentoCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors);
    }
}
