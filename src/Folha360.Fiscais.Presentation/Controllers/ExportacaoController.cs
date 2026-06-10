using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Fiscais.Presentation.Controllers;

[ApiController]
[Route("api/fiscais/exportacoes")]
[Authorize]
public class ExportacaoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportacaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{empresaId}/{periodo}")]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> ListarExportacoes(Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarExportacoesQuery(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{empresaId}/{periodo}/enviar-sftp")]
    [Authorize(Roles = "Contador,Admin")]
    public async Task<IActionResult> EnviarSftp(Guid empresaId, string periodo, CancellationToken ct)
    {
        var result = await _mediator.Send(new EnviarExportacaoSftpCommand(empresaId, periodo), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
