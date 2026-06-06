using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Repositories;

public class AfastamentoRepository : IAfastamentoRepository
{
    private readonly EventosDbContext _db;
    public AfastamentoRepository(EventosDbContext db) => _db = db;

    public async Task<Afastamento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Afastamentos.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Afastamento>> GetAllAsync(CancellationToken ct = default)
        => await _db.Afastamentos.ToListAsync(ct);

    public async Task<(IEnumerable<Afastamento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default)
    {
        var query = _db.Afastamentos.AsQueryable();

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

    public async Task AddAsync(Afastamento entity, CancellationToken ct = default)
    {
        _db.Afastamentos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Afastamento entity, CancellationToken ct = default)
    {
        _db.Afastamentos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Afastamentos.FindAsync([id], ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
