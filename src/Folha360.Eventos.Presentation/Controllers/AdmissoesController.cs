using Folha360.Application;
using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Eventos.Presentation.Controllers;

[ApiController]
[Route("api/admissoes")]
[Authorize(Policy = "Operador")]
public class AdmissoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdmissoesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<AdmissaoDto>> Criar([FromBody] CriarAdmissaoCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value)
            : UnprocessableEntity(result.Errors);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<AdmissaoDto>> Obter(Guid id)
    {
        var result = await _mediator.Send(new ObterAdmissaoQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<AdmissaoDto>>> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? funcionarioId = null)
    {
        var result = await _mediator.Send(new ListarAdmissoesQuery(page, pageSize, funcionarioId));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdmissaoDto>> Atualizar(Guid id, [FromBody] AtualizarAdmissaoCommand command)
    {
        var result = await _mediator.Send(command with { Id = id });
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var result = await _mediator.Send(new ExcluirAdmissaoCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Errors);
    }
}
