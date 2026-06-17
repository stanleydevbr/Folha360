using Folha360.Cadastros.Application;
using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Cadastros.Presentation.Controllers;

[Route("api/[controller]")]
public class CbosController : ControllerBase
{
    private readonly IMediator _mediator;
    public CbosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<CboDto>>> Listar(
        [FromQuery] ListarCbosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
public class NaturezasJuridicasController : ControllerBase
{
    private readonly IMediator _mediator;
    public NaturezasJuridicasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<NaturezaJuridicaDto>>> Listar(
        [FromQuery] ListarNaturezasJuridicasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
public class MunicipiosController : ControllerBase
{
    private readonly IMediator _mediator;
    public MunicipiosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<MunicipioIBGEDto>>> Listar(
        [FromQuery] ListarMunicipiosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BancosController : ControllerBase
{
    private readonly IMediator _mediator;
    public BancosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<BancoFebrabanDto>>> Listar(
        [FromQuery] ListarBancosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

// ============================
// Cadastros de Apoio (Sindicatos, Convênios, Horários, Dependentes, Documentos, Processos)
// ============================
