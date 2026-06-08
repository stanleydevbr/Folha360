using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using Folha360.Processamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Processamento.Infrastructure.Repositories;

public class CadeiaFechamentoRepository : ICadeiaFechamentoRepository
{
    private readonly ProcessamentoDbContext _db;

    public CadeiaFechamentoRepository(ProcessamentoDbContext db) => _db = db;

    public async Task<CadeiaFechamento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.CadeiasFechamento.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<CadeiaFechamento?> GetByEmpresaPeriodoAsync(
        Guid empresaId, DateOnly periodo, CancellationToken ct = default)
        => await _db.CadeiasFechamento
            .FirstOrDefaultAsync(c =>
                c.EmpresaId == empresaId &&
                c.Periodo == periodo &&
                c.DeletedAt == null, ct);

    public async Task AddAsync(CadeiaFechamento cadeia, CancellationToken ct = default)
    {
        _db.CadeiasFechamento.Add(cadeia);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CadeiaFechamento cadeia, CancellationToken ct = default)
    {
        _db.CadeiasFechamento.Update(cadeia);
        await _db.SaveChangesAsync(ct);
    }
}
