using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Abstractions;
using Folha360.Esocial.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Esocial.Infrastructure.Repositories;

public class EventoEsocialRepository : IEventoEsocialRepository
{
    private readonly Folha360DbContext _context;

    public EventoEsocialRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<EventoEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await _context.EventosEsocial.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<EventoEsocial>> ObterPendentesPorEmpresaAsync(Guid empresaId, int limite, CancellationToken ct = default)
        => await _context.EventosEsocial
            .Where(e => e.EmpresaId == empresaId && e.Status == StatusEvento.Pendente)
            .OrderBy(e => e.CreatedAt)
            .Take(limite)
            .ToListAsync(ct);

    public async Task<List<EventoEsocial>> ObterPorLoteAsync(Guid loteId, CancellationToken ct = default)
        => await _context.EventosEsocial
            .Where(e => e.LoteId == loteId)
            .ToListAsync(ct);

    public async Task AdicionarAsync(EventoEsocial evento, CancellationToken ct = default)
    {
        await _context.EventosEsocial.AddAsync(evento, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(EventoEsocial evento, CancellationToken ct = default)
    {
        _context.EventosEsocial.Update(evento);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> ContarPendentesPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _context.EventosEsocial
            .CountAsync(e => e.EmpresaId == empresaId && e.Status == StatusEvento.Pendente, ct);
}

public class LoteEsocialRepository : ILoteEsocialRepository
{
    private readonly Folha360DbContext _context;

    public LoteEsocialRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<LoteEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await _context.LotesEsocial.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<List<LoteEsocial>> ObterPorEmpresaAsync(Guid empresaId, DateTime? inicio = null, DateTime? fim = null, CancellationToken ct = default)
    {
        var query = _context.LotesEsocial.Where(l => l.EmpresaId == empresaId);

        if (inicio.HasValue)
            query = query.Where(l => l.CreatedAt >= inicio.Value);
        if (fim.HasValue)
            query = query.Where(l => l.CreatedAt <= fim.Value);

        return await query.OrderByDescending(l => l.CreatedAt).ToListAsync(ct);
    }

    public async Task<List<LoteEsocial>> ObterLotesEnviadosPendentesAsync(CancellationToken ct = default)
        => await _context.LotesEsocial
            .Where(l => l.Status == StatusLote.Enviado && l.DataEnvio != null)
            .OrderBy(l => l.DataEnvio)
            .ToListAsync(ct);

    public async Task AdicionarAsync(LoteEsocial lote, CancellationToken ct = default)
    {
        await _context.LotesEsocial.AddAsync(lote, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(LoteEsocial lote, CancellationToken ct = default)
    {
        _context.LotesEsocial.Update(lote);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<LoteEsocial?> ObterPorProtocoloAsync(string protocolo, CancellationToken ct = default)
        => await _context.LotesEsocial.FirstOrDefaultAsync(l => l.ProtocoloEnvio == protocolo, ct);
}

public class FalhaEsocialRepository : IFalhaEsocialRepository
{
    private readonly Folha360DbContext _context;

    public FalhaEsocialRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<FalhaEsocial?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await _context.FalhasEsocial.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<List<FalhaEsocial>> ObterNaoResolvidasAsync(CancellationToken ct = default)
        => await _context.FalhasEsocial
            .Where(f => f.ResolvidoEm == null)
            .OrderByDescending(f => f.DataUltimaTentativa)
            .ToListAsync(ct);

    public async Task AdicionarAsync(FalhaEsocial falha, CancellationToken ct = default)
    {
        await _context.FalhasEsocial.AddAsync(falha, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(FalhaEsocial falha, CancellationToken ct = default)
    {
        _context.FalhasEsocial.Update(falha);
        await _context.SaveChangesAsync(ct);
    }
}

public class CertificadoDigitalRepository : ICertificadoDigitalRepository
{
    private readonly Folha360DbContext _context;

    public CertificadoDigitalRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<CertificadoDigital?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CertificadosDigitais.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<CertificadoDigital?> ObterAtivoPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _context.CertificadosDigitais
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Ativo, ct);

    public async Task<List<CertificadoDigital>> ObterProximosExpiracaoAsync(int diasLimite, CancellationToken ct = default)
    {
        var dataLimite = DateTime.UtcNow.Date.AddDays(diasLimite);
        return await _context.CertificadosDigitais
            .Where(c => c.Ativo && c.DataExpiracao <= dataLimite)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(CertificadoDigital certificado, CancellationToken ct = default)
    {
        await _context.CertificadosDigitais.AddAsync(certificado, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(CertificadoDigital certificado, CancellationToken ct = default)
    {
        _context.CertificadosDigitais.Update(certificado);
        await _context.SaveChangesAsync(ct);
    }
}
