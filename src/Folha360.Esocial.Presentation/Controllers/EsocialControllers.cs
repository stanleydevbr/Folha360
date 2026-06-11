using Folha360.Application;
using Folha360.Esocial.Application.Commands;
using Folha360.Esocial.Application.DTOs;
using Folha360.Esocial.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Esocial.Presentation.Controllers;

[ApiController]
[Route("api/esocial")]
public class LoteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILoteEsocialRepository _loteRepo;
    private readonly IEventoEsocialRepository _eventoRepo;

    public LoteController(
        IMediator mediator,
        ILoteEsocialRepository loteRepo,
        IEventoEsocialRepository eventoRepo)
    {
        _mediator = mediator;
        _loteRepo = loteRepo;
        _eventoRepo = eventoRepo;
    }

    [HttpPost("lotes/enviar")]
    public async Task<ActionResult<Result<LoteEnvioResultDto>>> EnviarLote([FromBody] EnviarLoteCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("lotes/{empresaId}")]
    public async Task<ActionResult<List<LoteDto>>> ObterLotes(
        Guid empresaId,
        [FromQuery] DateTime? inicio = null,
        [FromQuery] DateTime? fim = null)
    {
        var lotes = await _loteRepo.ObterPorEmpresaAsync(empresaId, inicio, fim);
        var dtos = lotes.Select(l => new LoteDto(
            l.Id, l.EmpresaId, l.TipoAmbiente.ToString(), l.Status.ToString(),
            l.ProtocoloEnvio, l.QuantidadeEventos, l.DataEnvio, l.DataProcessamento, l.CreatedAt)).ToList();
        return Ok(dtos);
    }

    [HttpGet("lotes/{loteId}/eventos")]
    public async Task<ActionResult<List<EventoEsocialDto>>> ObterEventosPorLote(Guid loteId)
    {
        var eventos = await _eventoRepo.ObterPorLoteAsync(loteId);
        var dtos = eventos.Select(e => new EventoEsocialDto(
            e.Id, e.EmpresaId, e.FuncionarioId, e.TipoEvento.ToString(), e.Status.ToString(),
            e.IdEvento, e.LoteId, e.CreatedAt, e.ProcessadoEm)).ToList();
        return Ok(dtos);
    }
}

[ApiController]
[Route("api/esocial")]
public class FalhaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFalhaEsocialRepository _falhaRepo;

    public FalhaController(IMediator mediator, IFalhaEsocialRepository falhaRepo)
    {
        _mediator = mediator;
        _falhaRepo = falhaRepo;
    }

    [HttpPost("falhas/{falhaId}/reprocessar")]
    public async Task<ActionResult<Result<bool>>> ReprocessarFalha(Guid falhaId)
    {
        var result = await _mediator.Send(new ReprocessarFalhaCommand(falhaId));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("falhas")]
    public async Task<ActionResult<List<FalhaEsocialDto>>> ObterFalhas()
    {
        var falhas = await _falhaRepo.ObterNaoResolvidasAsync();
        var dtos = falhas.Select(f => new FalhaEsocialDto(
            f.Id, f.EventoId, f.TipoErro.ToString(), f.CodigoErro, f.MensagemErro,
            f.Tentativas, f.DataUltimaTentativa, f.ResolvidoEm)).ToList();
        return Ok(dtos);
    }
}

[ApiController]
[Route("api/esocial")]
public class CertificadoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICertificadoDigitalRepository _certificadoRepo;

    public CertificadoController(IMediator mediator, ICertificadoDigitalRepository certificadoRepo)
    {
        _mediator = mediator;
        _certificadoRepo = certificadoRepo;
    }

    [HttpPost("certificados")]
    public async Task<ActionResult<Result<CertificadoDto>>> UploadCertificado(
        [FromQuery] Guid empresaId,
        [FromBody] CertificadoUploadRequest request)
    {
        var command = new UploadCertificadoA1Command(
            empresaId,
            request.ArquivoPfx,
            request.Senha);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("certificados/a3/testar")]
    public async Task<ActionResult<Result<CertificadoDto>>> TestarCertificadoA3(
        [FromQuery] Guid empresaId,
        [FromBody] CertificadoA3TestRequest request)
    {
        var command = new TestarCertificadoA3Command(empresaId, request.Pin);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("certificados/status")]
    public async Task<ActionResult<CertificadoDto>> ObterStatusCertificado([FromQuery] Guid empresaId)
    {
        var certificado = await _certificadoRepo.ObterAtivoPorEmpresaAsync(empresaId);
        if (certificado == null)
            return NotFound();

        return Ok(new CertificadoDto(
            certificado.Id,
            certificado.Tipo.ToString(),
            certificado.Emitente,
            certificado.Cnpj,
            certificado.DataExpiracao,
            certificado.DiasRestantes,
            certificado.Ativo,
            certificado.EstaExpirado));
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", module = "esocial", timestamp = DateTime.UtcNow });
    }
}
