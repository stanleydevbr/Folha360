using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Fiscais.Infrastructure.Repositories;

public class RegraFiscalRepository : IRegraFiscalRepository
{
    private readonly FiscaisDbContext _context;

    public RegraFiscalRepository(FiscaisDbContext context)
    {
        _context = context;
    }

    public async Task<RegraFiscal?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.RegrasFiscais.FindAsync([id], ct);

    public async Task<RegraFiscal?> GetVigenteAsync(Tributo tributo, DateOnly data, CancellationToken ct = default)
        => await _context.RegrasFiscais
            .Where(r => r.Tributo == tributo
                && r.Ativo
                && r.VigenciaInicio <= data
                && (r.VigenciaFim == null || r.VigenciaFim >= data))
            .OrderByDescending(r => r.Versao)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<RegraFiscal>> GetByTributoAsync(Tributo tributo, CancellationToken ct = default)
        => await _context.RegrasFiscais
            .Where(r => r.Tributo == tributo)
            .OrderByDescending(r => r.Versao)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RegraFiscal>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.RegrasFiscais
            .OrderByDescending(r => r.Versao)
            .ToListAsync(ct);
    }

    public async Task AddAsync(RegraFiscal regra, CancellationToken ct = default)
    {
        await _context.RegrasFiscais.AddAsync(regra, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RegraFiscal regra, CancellationToken ct = default)
    {
        _context.RegrasFiscais.Update(regra);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Tributo tributo, int versao, CancellationToken ct = default)
        => await _context.RegrasFiscais
            .AnyAsync(r => r.Tributo == tributo && r.Versao == versao, ct);
}
