using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Eventos.Presentation.Controllers;

[ApiController]
[Route("api/eventos-trabalhistas")]
[Authorize(Policy = "Consulta")]
public class EventosTrabalhistasController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventosTrabalhistasController(IMediator mediator) => _mediator = mediator;

    [HttpGet("funcionario/{id:guid}")]
    public async Task<ActionResult<EventosFuncionarioDto>> ListarEventosFuncionario(Guid id)
    {
        var result = await _mediator.Send(new ListarEventosFuncionarioQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }
}
