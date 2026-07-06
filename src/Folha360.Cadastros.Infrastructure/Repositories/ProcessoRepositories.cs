using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Repositories;

// ============================
// Processos Administrativos (S-1070) and Rubrica-Processo (N:N)
// ============================
public class ProcessoAdministrativoRepository : IProcessoAdministrativoRepository
{
    private readonly CadastrosDbContext _db;
    public ProcessoAdministrativoRepository(CadastrosDbContext db) => _db = db;

    public async Task<ProcessoAdministrativo?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ProcessosAdministrativos
            .Include(p => p.RubricasProcesso)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<ProcessoAdministrativo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ProcessosAdministrativos
            .Where(p => p.EmpresaId == empresaId)
            .ToListAsync(ct);

    public async Task<(IEnumerable<ProcessoAdministrativo> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? tipo = null,
        CancellationToken ct = default)
    {
        var query = _db.ProcessosAdministrativos.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(p => p.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(p => p.Tipo == tipo);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(ProcessoAdministrativo entity, CancellationToken ct = default)
    {
        _db.ProcessosAdministrativos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ProcessoAdministrativo entity, CancellationToken ct = default)
    {
        _db.ProcessosAdministrativos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ProcessosAdministrativos.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ProcessosAdministrativos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class RubricaProcessoRepository : IRubricaProcessoRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaProcessoRepository(CadastrosDbContext db) => _db = db;

    public async Task<RubricaProcesso?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RubricasProcesso.FirstOrDefaultAsync(rp => rp.Id == id, ct);

    public async Task<IEnumerable<RubricaProcesso>> GetByRubricaIdAsync(Guid rubricaId, CancellationToken ct = default)
        => await _db.RubricasProcesso
            .Where(rp => rp.RubricaId == rubricaId)
            .Include(rp => rp.ProcessoAdministrativo)
            .ToListAsync(ct);

    public async Task<IEnumerable<RubricaProcesso>> GetByProcessoIdAsync(Guid processoAdministrativoId, CancellationToken ct = default)
        => await _db.RubricasProcesso
            .Where(rp => rp.ProcessoAdministrativoId == processoAdministrativoId)
            .Include(rp => rp.Rubrica)
            .ToListAsync(ct);

    public async Task AddAsync(RubricaProcesso entity, CancellationToken ct = default)
    {
        _db.RubricasProcesso.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.RubricasProcesso.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.RubricasProcesso.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
