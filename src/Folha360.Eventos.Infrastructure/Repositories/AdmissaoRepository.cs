using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Repositories;

public class AdmissaoRepository : IAdmissaoRepository
{
    private readonly EventosDbContext _db;
    public AdmissaoRepository(EventosDbContext db) => _db = db;

    public async Task<Admissao?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Admissoes.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Admissao>> GetAllAsync(CancellationToken ct = default)
        => await _db.Admissoes.ToListAsync(ct);

    public async Task<(IEnumerable<Admissao> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default)
    {
        var query = _db.Admissoes.AsQueryable();

        if (funcionarioId.HasValue)
            query = query.Where(e => e.FuncionarioId == funcionarioId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.DataAdmissao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Admissao entity, CancellationToken ct = default)
    {
        _db.Admissoes.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Admissao entity, CancellationToken ct = default)
    {
        _db.Admissoes.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Admissoes.FindAsync([id], ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
