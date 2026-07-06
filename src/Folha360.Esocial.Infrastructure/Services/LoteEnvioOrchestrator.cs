using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Abstractions;
using Folha360.Esocial.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Folha360.Esocial.Infrastructure.Services;

public class LoteEnvioOrchestrator : ILoteEnvioOrchestrator
{
    private readonly IEventoEsocialRepository _eventoRepo;
    private readonly ILoteEsocialRepository _loteRepo;
    private readonly IFalhaEsocialRepository _falhaRepo;
    private readonly ILogger<LoteEnvioOrchestrator> _logger;

    public LoteEnvioOrchestrator(
        IEventoEsocialRepository eventoRepo,
        ILoteEsocialRepository loteRepo,
        IFalhaEsocialRepository falhaRepo,
        ILogger<LoteEnvioOrchestrator> logger)
    {
        _eventoRepo = eventoRepo;
        _loteRepo = loteRepo;
        _falhaRepo = falhaRepo;
        _logger = logger;
    }

    public async Task<List<EventoEsocial>> ObterEventosPendentesAsync(Guid empresaId, int limite, CancellationToken ct)
    {
        return await _eventoRepo.ObterPendentesPorEmpresaAsync(empresaId, limite, ct);
    }

    public async Task<LoteEsocial> CriarLoteAsync(Guid empresaId, TipoAmbiente ambiente, int quantidadeEventos, CancellationToken ct)
    {
        var lote = new LoteEsocial(empresaId, ambiente);
        lote.IniciarAssinatura(quantidadeEventos);
        await _loteRepo.AdicionarAsync(lote, ct);
        return lote;
    }

    public async Task RegistrarEventoAssinadoAsync(EventoEsocial evento, Guid certificadoId, string hash, Guid loteId, CancellationToken ct)
    {
        evento.Assinar(certificadoId, hash);
        evento.Enviar(loteId);
        await _eventoRepo.AtualizarAsync(evento, ct);
    }

    public async Task RegistrarEventoComErroAsync(EventoEsocial evento, Exception ex, CancellationToken ct)
    {
        evento.MarcarErro();
        await _eventoRepo.AtualizarAsync(evento, ct);

        var falha = new FalhaEsocial(
            evento.Id,
            TipoErroEsocial.Assinatura,
            ex.Message,
            loteId: evento.LoteId);
        await _falhaRepo.AdicionarAsync(falha, ct);

        _logger.LogError(ex, "Erro ao processar evento {EventoId}", evento.Id);
    }

    public async Task ConcluirLoteAsync(LoteEsocial lote, CancellationToken ct)
    {
        lote.ConcluirAssinatura();
        await _loteRepo.AtualizarAsync(lote, ct);
    }

    public async Task RegistrarEnvioLoteAsync(LoteEsocial lote, string protocolo, CancellationToken ct)
    {
        lote.Enviar(protocolo);
        await _loteRepo.AtualizarAsync(lote, ct);
    }

    public async Task RegistrarErroLoteAsync(LoteEsocial lote, CancellationToken ct)
    {
        lote.MarcarErro();
        await _loteRepo.AtualizarAsync(lote, ct);
    }
}
