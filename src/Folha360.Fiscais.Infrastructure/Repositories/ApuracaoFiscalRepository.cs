using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Fiscais.Infrastructure.Repositories;

public class ApuracaoFiscalRepository : IApuracaoFiscalRepository
{
    private readonly FiscaisDbContext _context;

    public ApuracaoFiscalRepository(FiscaisDbContext context)
    {
        _context = context;
    }

    public async Task<ApuracaoFiscal?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ApuracoesFiscais.FindAsync([id], ct);

    public async Task<IReadOnlyList<ApuracaoFiscal>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default)
        => await _context.ApuracoesFiscais
            .Where(a => a.EmpresaId == empresaId && a.Periodo == periodo)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ApuracaoFiscal>> GetByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default)
        => await _context.ApuracoesFiscais
            .Where(a => a.ProcessamentoId == processamentoId)
            .ToListAsync(ct);

    public async Task AddAsync(ApuracaoFiscal apuracao, CancellationToken ct = default)
        => await _context.ApuracoesFiscais.AddAsync(apuracao, ct);

    public Task UpdateAsync(ApuracaoFiscal apuracao, CancellationToken ct = default)
    {
        _context.ApuracoesFiscais.Update(apuracao);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid empresaId, DateOnly periodo, Tributo tributo, Guid processamentoId, CancellationToken ct = default)
        => await _context.ApuracoesFiscais
            .AnyAsync(a => a.EmpresaId == empresaId
                && a.Periodo == periodo
                && a.Tributo == tributo
                && a.ProcessamentoId == processamentoId
                && a.DeletedAt == null, ct);
}
