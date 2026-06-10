using Folha360.Application;
using Folha360.Processamento.Application.Commands;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Processamento.Presentation.Controllers;

[ApiController]
[Route("api/folha")]
[Authorize(Policy = "Operador")]
public class ProcessamentoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcessamentoController(IMediator mediator) => _mediator = mediator;

    /// <summary>Iniciar processamento da folha (RF12)</summary>
    [HttpPost("processar")]
    [ProducesResponseType(typeof(ProcessamentoResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ProcessamentoResponse>> Processar(
        [FromBody] IniciarProcessamentoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code == "CONFLITO"))
                return Conflict(new { result.Errors });
            return UnprocessableEntity(new { result.Errors });
        }

        return AcceptedAtAction(
            nameof(ObterProcessamento),
            new { id = result.Value!.Id },
            result.Value);
    }

    /// <summary>Obter status do processamento (RF15)</summary>
    [HttpGet("processamento/{id:guid}")]
    [Authorize(Policy = "Consulta")]
    [ProducesResponseType(typeof(ProcessamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessamentoResponse>> ObterProcessamento(
        Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterProcessamentoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    /// <summary>Cancelar processamento (RF16)</summary>
    [HttpPost("processamento/{id:guid}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelarProcessamento(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelarProcessamentoCommand { ProcessamentoId = id }, ct);
        return result.IsSuccess ? Ok() : NotFound(new { result.Errors });
    }

    /// <summary>Reprocessar folha (RF44)</summary>
    [HttpPost("reprocessar")]
    [ProducesResponseType(typeof(ProcessamentoResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessamentoResponse>> Reprocessar(
        [FromBody] ReprocessarFolhaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return NotFound(new { result.Errors });

        return Accepted(result.Value);
    }

    /// <summary>Obter itens da folha</summary>
    [HttpGet("processamento/{id:guid}/itens")]
    [Authorize(Policy = "Consulta")]
    [ProducesResponseType(typeof(List<ItemFolhaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ItemFolhaResponse>>> ObterItens(
        Guid id, [FromQuery] Guid? funcionarioId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterItensFolhaQuery
        {
            ProcessamentoId = id,
            FuncionarioId = funcionarioId,
        }, ct);

        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    /// <summary>Reabrir processamento (RF46)</summary>
    [HttpPost("{id:guid}/reabrir")]
    [ProducesResponseType(typeof(ReaberturaStatusResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ReaberturaStatusResponse>> Reabrir(
        Guid id, [FromBody] ReabrirProcessamentoRequest request, CancellationToken ct)
    {
        var command = new ReabrirProcessamentoCommand
        {
            ProcessamentoId = id,
            Motivo = request.Motivo,
            Autor = request.Autor,
        };

        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Code == "CONFLITO"))
                return Conflict(new { result.Errors });
            return UnprocessableEntity(new { result.Errors });
        }

        return Accepted(result.Value);
    }

    /// <summary>Status da reabertura (RF48)</summary>
    [HttpGet("{id:guid}/reabertura/status")]
    [Authorize(Policy = "Consulta")]
    [ProducesResponseType(typeof(ReaberturaStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReaberturaStatusResponse>> ObterReaberturaStatus(
        Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterReaberturaStatusQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    /// <summary>Histórico de versões de processamento (RF52)</summary>
    [HttpGet("{empresaId:guid}/{periodo}/historico")]
    [Authorize(Policy = "Consulta")]
    [ProducesResponseType(typeof(List<HistoricoProcessamentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<HistoricoProcessamentoResponse>>> ObterHistorico(
        Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ObterHistoricoProcessamentoQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    /// <summary>Status da cadeia de fechamento (RF35)</summary>
    [HttpGet("fechamento/{empresaId:guid}/{periodo}")]
    [Authorize(Policy = "Consulta")]
    [ProducesResponseType(typeof(ReaberturaStatusResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReaberturaStatusResponse>> ObterFechamento(
        Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ObterFechamentoStatusQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }
}
