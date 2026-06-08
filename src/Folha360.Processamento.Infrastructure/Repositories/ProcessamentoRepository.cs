using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using Folha360.Processamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Processamento.Infrastructure.Repositories;

public class ProcessamentoRepository : IProcessamentoRepository
{
    private readonly ProcessamentoDbContext _db;

    public ProcessamentoRepository(ProcessamentoDbContext db) => _db = db;

    public async Task<ProcessamentoFolha?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ProcessamentosFolha.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<ProcessamentoFolha?> GetByEmpresaPeriodoAsync(
        Guid empresaId, DateOnly periodo, TipoCalculo tipoCalculo, int versao, CancellationToken ct = default)
        => await _db.ProcessamentosFolha
            .FirstOrDefaultAsync(p =>
                p.EmpresaId == empresaId &&
                p.Periodo == periodo &&
                p.TipoCalculo == tipoCalculo &&
                p.Versao == versao &&
                p.DeletedAt == null, ct);

    public async Task<IEnumerable<ProcessamentoFolha>> GetHistoricoAsync(
        Guid empresaId, DateOnly periodo, CancellationToken ct = default)
        => await _db.ProcessamentosFolha
            .IgnoreQueryFilters()
            .Where(p => p.EmpresaId == empresaId && p.Periodo == periodo)
            .OrderByDescending(p => p.Versao)
            .ToListAsync(ct);

    public async Task<(IEnumerable<ProcessamentoFolha> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? empresaId = null, DateOnly? periodo = null,
        StatusProcessamento? status = null, TipoCalculo? tipoCalculo = null,
        CancellationToken ct = default)
    {
        var query = _db.ProcessamentosFolha.AsQueryable();

        if (empresaId.HasValue)
            query = query.Where(p => p.EmpresaId == empresaId.Value);
        if (periodo.HasValue)
            query = query.Where(p => p.Periodo == periodo.Value);
        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);
        if (tipoCalculo.HasValue)
            query = query.Where(p => p.TipoCalculo == tipoCalculo.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.DataInicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(ProcessamentoFolha processamento, CancellationToken ct = default)
    {
        _db.ProcessamentosFolha.Add(processamento);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ProcessamentoFolha processamento, CancellationToken ct = default)
    {
        _db.ProcessamentosFolha.Update(processamento);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ProcessamentosFolha.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ProcessamentosFolha.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
