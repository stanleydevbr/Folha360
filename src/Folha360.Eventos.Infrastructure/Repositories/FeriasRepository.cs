using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Repositories;

public class FeriasRepository : IFeriasRepository
{
    private readonly EventosDbContext _db;
    public FeriasRepository(EventosDbContext db) => _db = db;

    public async Task<Ferias?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Ferias.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Ferias>> GetAllAsync(CancellationToken ct = default)
        => await _db.Ferias.ToListAsync(ct);

    public async Task<(IEnumerable<Ferias> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default)
    {
        var query = _db.Ferias.AsQueryable();

        if (funcionarioId.HasValue)
            query = query.Where(e => e.FuncionarioId == funcionarioId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.DataInicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Ferias entity, CancellationToken ct = default)
    {
        _db.Ferias.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ferias entity, CancellationToken ct = default)
    {
        _db.Ferias.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Ferias.FindAsync([id], ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
