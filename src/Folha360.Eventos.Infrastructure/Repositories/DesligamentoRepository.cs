using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Repositories;

public class DesligamentoRepository : IDesligamentoRepository
{
    private readonly EventosDbContext _db;
    public DesligamentoRepository(EventosDbContext db) => _db = db;

    public async Task<Desligamento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Desligamentos.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Desligamento>> GetAllAsync(CancellationToken ct = default)
        => await _db.Desligamentos.ToListAsync(ct);

    public async Task<(IEnumerable<Desligamento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default)
    {
        var query = _db.Desligamentos.AsQueryable();

        if (funcionarioId.HasValue)
            query = query.Where(e => e.FuncionarioId == funcionarioId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.DataDesligamento)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Desligamento entity, CancellationToken ct = default)
    {
        _db.Desligamentos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Desligamento entity, CancellationToken ct = default)
    {
        _db.Desligamentos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Desligamentos.FindAsync([id], ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
