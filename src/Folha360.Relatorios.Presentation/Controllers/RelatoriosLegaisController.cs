using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
public class RelatoriosLegaisController : ControllerBase
{
    private readonly IMediator _mediator;

    public RelatoriosLegaisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dirf/{empresaId:guid}/{ano:int}")]
    [Authorize(Policy = "Contador")]
    public async Task<IActionResult> ObterDirf(Guid empresaId, int ano, [FromQuery] string? formato = "json")
    {
        var query = new ObterDirfQuery { EmpresaId = empresaId, Ano = ano };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return await FormatarRespostaAsync(result.Value!, "dirf", empresaId, ano, formato);
    }

    [HttpGet("rais/{empresaId:guid}/{ano:int}")]
    [Authorize(Policy = "Contador")]
    public async Task<IActionResult> ObterRais(Guid empresaId, int ano, [FromQuery] string? formato = "json")
    {
        var query = new ObterRaisQuery { EmpresaId = empresaId, Ano = ano };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return await FormatarRespostaAsync(result.Value!, "rais", empresaId, ano, formato);
    }

    private async Task<IActionResult> FormatarRespostaAsync<T>(IReadOnlyList<T> dados, string tipo, Guid empresaId, int ano, string? formato)
    {
        switch (formato?.ToLower())
        {
            case "csv":
                {
                    var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                    Stream stream;
                    if (tipo == "dirf")
                    {
                        stream = await exportService.ExportarCsvDirfAsync(dados.Cast<DirfDto>().ToList(), HttpContext.RequestAborted);
                    }
                    else
                    {
                        stream = await exportService.ExportarCsvRaisAsync(dados.Cast<RaisDto>().ToList(), HttpContext.RequestAborted);
                    }

                    return File(stream, "text/csv", $"{tipo}_{empresaId}_{ano}.csv");
                }

            case "xml":
                {
                    var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                    var stream = await exportService.ExportarXmlAsync(dados, tipo, HttpContext.RequestAborted);
                    return File(stream, "application/xml", $"{tipo}_{empresaId}_{ano}.xml");
                }

            default:
                return Ok(dados);
        }
    }
}
