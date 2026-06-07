using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Eventos.Infrastructure.Repositories;

public class AlteracaoContratualRepository : IAlteracaoContratualRepository
{
    private readonly EventosDbContext _db;
    public AlteracaoContratualRepository(EventosDbContext db) => _db = db;

    public async Task<AlteracaoContratual?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.AlteracoesContratuais.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<AlteracaoContratual>> GetAllAsync(CancellationToken ct = default)
        => await _db.AlteracoesContratuais.ToListAsync(ct);

    public async Task<(IEnumerable<AlteracaoContratual> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default)
    {
        var query = _db.AlteracoesContratuais.AsQueryable();

        if (funcionarioId.HasValue)
            query = query.Where(e => e.FuncionarioId == funcionarioId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.DataAlteracao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(AlteracaoContratual entity, CancellationToken ct = default)
    {
        _db.AlteracoesContratuais.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AlteracaoContratual entity, CancellationToken ct = default)
    {
        _db.AlteracoesContratuais.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.AlteracoesContratuais.FindAsync([id], ct);
        if (entity is not null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
