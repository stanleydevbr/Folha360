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

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class CargosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CargosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<CargoDto>>> Listar(
        [FromQuery] ListarCargosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<CargoDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterCargoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<CargoDto>> Criar(
        [FromBody] CriarCargoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CargoDto>> Atualizar(
        Guid id, [FromBody] AtualizarCargoCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirCargoCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[Route("api/[controller]")]
[Authorize(Policy = "Operador")]

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class LotacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LotacoesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<LotacaoDto>>> Listar(
        [FromQuery] ListarLotacoesQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<LotacaoDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterLotacaoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<LotacaoDto>> Criar(
        [FromBody] CriarLotacaoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LotacaoDto>> Atualizar(
        Guid id, [FromBody] AtualizarLotacaoCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirLotacaoCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

// ============================
