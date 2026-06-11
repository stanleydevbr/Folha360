using Folha360.Application;
using Folha360.Esocial.Application.Commands;
using Folha360.Esocial.Application.DTOs;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Abstractions;
using Folha360.Esocial.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Esocial.Application.Handlers;

public class EnviarLoteCommandHandler : IRequestHandler<EnviarLoteCommand, Result<LoteEnvioResultDto>>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILoteEsocialRepository _loteRepo;
    private readonly IXmlAssinaturaService _assinaturaService;
    private readonly IEsocialEnvioService _envioService;
    private readonly ICertificadoDigitalRepository _certificadoRepo;
    private readonly ILogger<EnviarLoteCommandHandler> _logger;

    public EnviarLoteCommandHandler(
        IEventoEsocialRepository eventoRepo,
        ILoteEsocialRepository loteRepo,
        IXmlAssinaturaService assinaturaService,
        IEsocialEnvioService envioService,
        ICertificadoDigitalRepository certificadoRepo,
        ILogger<EnviarLoteCommandHandler> logger)
    {
        _eventoRepo = eventoRepo;
        _loteRepo = loteRepo;
        _assinaturaService = assinaturaService;
        _envioService = envioService;
        _certificadoRepo = certificadoRepo;
        _logger = logger;
    }

    public async Task<Result<LoteEnvioResultDto>> Handle(EnviarLoteCommand request, CancellationToken ct)
    {
        var ambiente = Enum.Parse<TipoAmbiente>(request.TipoAmbiente);

        // Buscar certificado ativo
        var certificado = await _certificadoRepo.ObterAtivoPorEmpresaAsync(request.EmpresaId, ct);
        if (certificado == null)
            return Result<LoteEnvioResultDto>.Failure("CERT_NAO_ENCONTRADO", "Nenhum certificado digital ativo encontrado.");

        if (certificado.EstaExpirado)
            return Result<LoteEnvioResultDto>.Failure("CERT_EXPIRADO", "Certificado digital expirado.");

        // Buscar eventos pendentes
        var eventos = await _eventoRepo.ObterPendentesPorEmpresaAsync(request.EmpresaId, 100, ct);
        if (eventos.Count == 0)
            return Result<LoteEnvioResultDto>.Failure("SEM_EVENTOS", "Nenhum evento pendente para envio.");

        // Criar lote
        var lote = new LoteEsocial(request.EmpresaId, ambiente);
        lote.IniciarAssinatura(eventos.Count);
        await _loteRepo.AdicionarAsync(lote, ct);

        // Assinar eventos
        foreach (var evento in eventos)
        {
            try
            {
                evento.Validar();
                var xmlAssinado = await _assinaturaService.AssinarXmlAsync(evento.XmlConteudo, certificado, null, ct);
                evento.Assinar(certificado.Id, xmlAssinado.GetHashCode().ToString("X"));
                evento.Enviar(lote.Id);
                await _eventoRepo.AtualizarAsync(evento, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao assinar evento {EventoId}", evento.Id);
                evento.MarcarErro();
                await _eventoRepo.AtualizarAsync(evento, ct);
            }
        }

        lote.ConcluirAssinatura();
        await _loteRepo.AtualizarAsync(lote, ct);

        // Enviar lote
        try
        {
            var eventosAssinados = eventos.Where(e => e.Status == StatusEvento.Enviado).ToList();
            var protocolo = await _envioService.EnviarLoteAsync(lote, eventosAssinados, ct);
            lote.Enviar(protocolo);
            await _loteRepo.AtualizarAsync(lote, ct);

            return Result<LoteEnvioResultDto>.Success(new LoteEnvioResultDto(
                lote.Id, protocolo, eventosAssinados.Count, lote.Status.ToString()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar lote {LoteId}", lote.Id);
            lote.MarcarErro();
            await _loteRepo.AtualizarAsync(lote, ct);

            return Result<LoteEnvioResultDto>.Failure("ERRO_ENVIO", $"Erro ao enviar lote: {ex.Message}");
        }
    }
}

public class ReprocessarFalhaCommandHandler : IRequestHandler<ReprocessarFalhaCommand, Result<bool>>
{
    private readonly IFalhaEsocialRepository _falhaRepo;
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<ReprocessarFalhaCommandHandler> _logger;

    public ReprocessarFalhaCommandHandler(
        IFalhaEsocialRepository falhaRepo,
        IEventoEsocialRepository eventoRepo,
        ILogger<ReprocessarFalhaCommandHandler> logger)
    {
        _falhaRepo = falhaRepo;
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ReprocessarFalhaCommand request, CancellationToken ct)
    {
        var falha = await _falhaRepo.ObterPorIdAsync(request.FalhaId, ct);
        if (falha == null)
            return Result<bool>.Failure("FALHA_NAO_ENCONTRADA", "Falha não encontrada.");

        if (falha.ResolvidoEm.HasValue)
            return Result<bool>.Failure("FALHA_JA_RESOLVIDA", "Falha já foi resolvida.");

        var evento = await _eventoRepo.ObterPorIdAsync(falha.EventoId, ct);
        if (evento == null)
            return Result<bool>.Failure("EVENTO_NAO_ENCONTRADO", "Evento não encontrado.");

        falha.IncrementarTentativa();
        await _falhaRepo.AtualizarAsync(falha, ct);

        _logger.LogInformation("Falha {FalhaId} reprocessada. Tentativa {Tentativa}", falha.Id, falha.Tentativas);

        return Result<bool>.Success(true);
    }
}

public class UploadCertificadoA1CommandHandler : IRequestHandler<UploadCertificadoA1Command, Result<CertificadoDto>>
{
    private readonly ICertificadoDigitalRepository _certificadoRepo;
    private readonly ILogger<UploadCertificadoA1CommandHandler> _logger;

    public UploadCertificadoA1CommandHandler(
        ICertificadoDigitalRepository certificadoRepo,
        ILogger<UploadCertificadoA1CommandHandler> logger)
    {
        _certificadoRepo = certificadoRepo;
        _logger = logger;
    }

    public async Task<Result<CertificadoDto>> Handle(UploadCertificadoA1Command request, CancellationToken ct)
    {
        try
        {
            using var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(
                request.ArquivoPfx, request.Senha);

            var certificado = new CertificadoDigital(
                request.EmpresaId,
                TipoCertificado.A1,
                cert.Issuer,
                cert.GetNameInfo(System.Security.Cryptography.X509Certificates.X509NameType.SimpleName, false),
                cert.NotAfter,
                arquivoPfx: request.ArquivoPfx);

            await _certificadoRepo.AdicionarAsync(certificado, ct);

            return Result<CertificadoDto>.Success(new CertificadoDto(
                certificado.Id,
                certificado.Tipo.ToString(),
                certificado.Emitente,
                certificado.Cnpj,
                certificado.DataExpiracao,
                certificado.DiasRestantes,
                certificado.Ativo,
                certificado.EstaExpirado));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do certificado A1.");
            return Result<CertificadoDto>.Failure("CERT_INVALIDO", $"Certificado inválido: {ex.Message}");
        }
    }
}

public class EnviarEventosEsocialCommandHandler : IRequestHandler<EnviarEventosEsocialCommand, Result<bool>>
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILogger<EnviarEventosEsocialCommandHandler> _logger;

    public EnviarEventosEsocialCommandHandler(
        IEventoEsocialRepository eventoRepo,
        ILogger<EnviarEventosEsocialCommandHandler> logger)
    {
        _eventoRepo = eventoRepo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(EnviarEventosEsocialCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Enviando eventos e-Social: Empresa={EmpresaId}, Periodo={Periodo}",
            request.EmpresaId, request.Periodo);

        var pendentes = await _eventoRepo.ContarPendentesPorEmpresaAsync(request.EmpresaId, ct);
        _logger.LogInformation("{Count} eventos pendentes para envio.", pendentes);

        return Result<bool>.Success(true);
    }
}

public class ReverterEventosEsocialCommandHandler : IRequestHandler<ReverterEventosEsocialCommand, Result<bool>>
{
    private readonly ILogger<ReverterEventosEsocialCommandHandler> _logger;

    public ReverterEventosEsocialCommandHandler(ILogger<ReverterEventosEsocialCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(ReverterEventosEsocialCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Revertendo eventos e-Social: Empresa={EmpresaId}, Periodo={Periodo}",
            request.EmpresaId, request.Periodo);

        return Result<bool>.Success(true);
    }
}
