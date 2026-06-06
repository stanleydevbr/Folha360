using Folha360.Cadastros.Application;
using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Eventos.Presentation.Controllers;

[ApiController]
[Route("api/alteracoes-contratuais")]
[Authorize(Policy = "Operador")]
public class AlteracoesContratuaisController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlteracoesContratuaisController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<AlteracaoContratualDto>> Criar([FromBody] CriarAlteracaoContratualCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value)
            : UnprocessableEntity(result.Errors);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<AlteracaoContratualDto>> Obter(Guid id)
    {
        var result = await _mediator.Send(new ObterAlteracaoContratualQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<AlteracaoContratualDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? funcionarioId = null)
    {
        var result = await _mediator.Send(new ListarAlteracoesContratuaisQuery(page, pageSize, funcionarioId));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AlteracaoContratualDto>> Atualizar(Guid id, [FromBody] AtualizarAlteracaoContratualCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var result = await _mediator.Send(new ExcluirAlteracaoContratualCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors);
    }
}
