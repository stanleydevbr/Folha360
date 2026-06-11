using Folha360.Esocial.Domain.Entities;

namespace Folha360.Esocial.Domain.Abstractions;

public interface IEventoEsocialRepository
{
    Task<EventoEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<EventoEsocial>> ObterPendentesPorEmpresaAsync(Guid empresaId, int limite, CancellationToken ct = default);
    Task<List<EventoEsocial>> ObterPorLoteAsync(Guid loteId, CancellationToken ct = default);
    Task AdicionarAsync(EventoEsocial evento, CancellationToken ct = default);
    Task AtualizarAsync(EventoEsocial evento, CancellationToken ct = default);
    Task<int> ContarPendentesPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
}

public interface ILoteEsocialRepository
{
    Task<LoteEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<LoteEsocial>> ObterPorEmpresaAsync(Guid empresaId, DateTime? inicio = null, DateTime? fim = null, CancellationToken ct = default);
    Task<List<LoteEsocial>> ObterLotesEnviadosPendentesAsync(CancellationToken ct = default);
    Task AdicionarAsync(LoteEsocial lote, CancellationToken ct = default);
    Task AtualizarAsync(LoteEsocial lote, CancellationToken ct = default);
    Task<LoteEsocial?> ObterPorProtocoloAsync(string protocolo, CancellationToken ct = default);
}

public interface IFalhaEsocialRepository
{
    Task<FalhaEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<List<FalhaEsocial>> ObterNaoResolvidasAsync(CancellationToken ct = default);
    Task AdicionarAsync(FalhaEsocial falha, CancellationToken ct = default);
    Task AtualizarAsync(FalhaEsocial falha, CancellationToken ct = default);
}

public interface ICertificadoDigitalRepository
{
    Task<CertificadoDigital?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<CertificadoDigital?> ObterAtivoPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<List<CertificadoDigital>> ObterProximosExpiracaoAsync(int diasLimite, CancellationToken ct = default);
    Task AdicionarAsync(CertificadoDigital certificado, CancellationToken ct = default);
    Task AtualizarAsync(CertificadoDigital certificado, CancellationToken ct = default);
}
