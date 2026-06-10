using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Relatorios.Presentation.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Policy = "Contador")]
public class ResumoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("resumo-mensal/{empresaId:guid}/{periodo}")]
    public async Task<IActionResult> ObterResumoMensal(Guid empresaId, string periodo, [FromQuery] string? formato = "json")
    {
        var query = new ObterResumoMensalQuery { EmpresaId = empresaId, Periodo = periodo };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return FormatarResposta(result.Value!, "resumo_mensal", empresaId, periodo, formato);
    }

    [HttpGet("resumo-anual/{empresaId:guid}/{ano:int}")]
    public async Task<IActionResult> ObterResumoAnual(Guid empresaId, int ano, [FromQuery] string? formato = "json")
    {
        var query = new ObterResumoAnualQuery { EmpresaId = empresaId, Ano = ano };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return FormatarResposta(result.Value!, "resumo_anual", empresaId, ano.ToString(), formato);
    }

    private IActionResult FormatarResposta<T>(T dados, string tipo, Guid empresaId, string periodo, string? formato)
    {
        return formato?.ToLower() switch
        {
            "csv" => Task.Run(async () =>
            {
                var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                var list = new List<T> { dados };
                var stream = await exportService.ExportarCsvAsync(list.AsReadOnly(), HttpContext.RequestAborted);
                return File(stream, "text/csv", $"{tipo}_{empresaId}_{periodo}.csv");
            }).Result,
            "xml" => Task.Run(async () =>
            {
                var exportService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioExportService>();
                var list = new List<T> { dados };
                var stream = await exportService.ExportarXmlAsync(list.AsReadOnly(), tipo, HttpContext.RequestAborted);
                return File(stream, "application/xml", $"{tipo}_{empresaId}_{periodo}.xml");
            }).Result,
            "pdf" => Task.Run(async () =>
            {
                var pdfService = HttpContext.RequestServices.GetRequiredService<Folha360.Relatorios.Application.Services.IRelatorioPdfService>();
                Stream stream = dados switch
                {
                    ResumoMensalDto rm => await pdfService.GerarResumoMensalPdfAsync(rm, HttpContext.RequestAborted),
                    ResumoAnualDto ra => await pdfService.GerarResumoAnualPdfAsync(ra, HttpContext.RequestAborted),
                    _ => throw new NotSupportedException("Formato PDF não suportado para este tipo."),
                };
                return File(stream, "application/pdf", $"{tipo}_{empresaId}_{periodo}.pdf");
            }).Result,
            _ => Ok(dados),
        };
    }
}
