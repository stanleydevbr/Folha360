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
public class EmpresasController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmpresasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<EmpresaDto>>> Listar(
        [FromQuery] ListarEmpresasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<EmpresaDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterEmpresaQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaDto>> Criar(
        [FromBody] CriarEmpresaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmpresaDto>> Atualizar(
        Guid id, [FromBody] AtualizarEmpresaCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirEmpresaCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class FuncionariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public FuncionariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<FuncionarioDto>>> Listar(
        [FromQuery] ListarFuncionariosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<FuncionarioDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterFuncionarioQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<FuncionarioDto>> Criar(
        [FromBody] CriarFuncionarioCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FuncionarioDto>> Atualizar(
        Guid id, [FromBody] AtualizarFuncionarioCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirFuncionarioCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}
