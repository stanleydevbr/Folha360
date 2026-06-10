using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using Folha360.Processamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Processamento.Infrastructure.Repositories;

public class HoleriteRepository : IHoleriteRepository
{
    private readonly ProcessamentoDbContext _db;

    public HoleriteRepository(ProcessamentoDbContext db) => _db = db;

    public async Task<Holerite?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Holerites.FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<IEnumerable<Holerite>> GetByProcessamentoAsync(
        Guid processamentoId, CancellationToken ct = default)
        => await _db.Holerites
            .Where(h => h.ProcessamentoId == processamentoId)
            .ToListAsync(ct);

    public async Task<Holerite?> GetByFuncionarioAsync(
        Guid processamentoId, Guid funcionarioId, CancellationToken ct = default)
        => await _db.Holerites
            .FirstOrDefaultAsync(h =>
                h.ProcessamentoId == processamentoId &&
                h.FuncionarioId == funcionarioId, ct);

    public async Task AddAsync(Holerite holerite, CancellationToken ct = default)
    {
        _db.Holerites.Add(holerite);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddBatchAsync(IEnumerable<Holerite> holerites, CancellationToken ct = default)
    {
        _db.Holerites.AddRange(holerites);
        await _db.SaveChangesAsync(ct);
    }
}
