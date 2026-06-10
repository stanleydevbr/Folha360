using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Policy = "Operador")]
public class FolhaController : ControllerBase
{
    private readonly IMediator _mediator;

    public FolhaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("folha-analitica/{empresaId:guid}/{periodo}")]
    public async Task<IActionResult> ObterFolhaAnalitica(
        Guid empresaId, string periodo,
        [FromQuery] Guid? departamentoId,
        [FromQuery] string? tipoCalculo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? formato = "json")
    {
        var query = new ObterFolhaAnaliticaQuery
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            DepartamentoId = departamentoId,
            TipoCalculo = tipoCalculo,
            Page = page,
            PageSize = pageSize,
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return await FormatarRespostaAsync(result.Value!, "folha_analitica", empresaId, periodo, formato);
    }

    [HttpGet("folha-sintetica/{empresaId:guid}/{periodo}")]
    public async Task<IActionResult> ObterFolhaSintetica(
        Guid empresaId, string periodo,
        [FromQuery] Guid? departamentoId,
        [FromQuery] string? formato = "json")
    {
        var query = new ObterFolhaSinteticaQuery
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            DepartamentoId = departamentoId,
        };

        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return await FormatarRespostaAsync(result.Value!, "folha_sintetica", empresaId, periodo, formato);
    }

    private async Task<IActionResult> FormatarRespostaAsync<T>(T dados, string tipo, Guid empresaId, string periodo, string? formato)
    {
        switch (formato?.ToLower())
        {
            case "csv":
                {
                    var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                    var list = new List<T> { dados };
                    var stream = await exportService.ExportarCsvAsync(list.AsReadOnly(), HttpContext.RequestAborted);
                    return File(stream, "text/csv", $"{tipo}_{empresaId}_{periodo}.csv");
                }

            case "xml":
                {
                    var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                    var list = new List<T> { dados };
                    var stream = await exportService.ExportarXmlAsync(list.AsReadOnly(), tipo, HttpContext.RequestAborted);
                    return File(stream, "application/xml", $"{tipo}_{empresaId}_{periodo}.xml");
                }

            case "pdf":
                {
                    var pdfService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioPdfService>();
                    Stream stream = dados switch
                    {
                        FolhaAnaliticaDto fa => await pdfService.GerarFolhaAnaliticaPdfAsync(fa, HttpContext.RequestAborted),
                        FolhaSinteticaDto fs => await pdfService.GerarFolhaSinteticaPdfAsync(fs, HttpContext.RequestAborted),
                        _ => throw new NotSupportedException("Formato PDF não suportado para este tipo."),
                    };
                    return File(stream, "application/pdf", $"{tipo}_{empresaId}_{periodo}.pdf");
                }

            default:
                return Ok(dados);
        }
    }
}
