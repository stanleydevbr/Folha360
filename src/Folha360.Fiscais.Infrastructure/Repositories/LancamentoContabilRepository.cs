using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Fiscais.Infrastructure.Repositories;

public class LancamentoContabilRepository : ILancamentoContabilRepository
{
    private readonly FiscaisDbContext _context;

    public LancamentoContabilRepository(FiscaisDbContext context)
    {
        _context = context;
    }

    public async Task<LancamentoContabil?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.LancamentosContabeis.FindAsync([id], ct);

    public async Task<IReadOnlyList<LancamentoContabil>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default)
        => await _context.LancamentosContabeis
            .Where(l => l.EmpresaId == empresaId && l.Periodo == periodo)
            .ToListAsync(ct);

    public async Task AddAsync(LancamentoContabil lancamento, CancellationToken ct = default)
    {
        await _context.LancamentosContabeis.AddAsync(lancamento, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(LancamentoContabil lancamento, CancellationToken ct = default)
    {
        _context.LancamentosContabeis.Update(lancamento);
        await _context.SaveChangesAsync(ct);
    }
}
