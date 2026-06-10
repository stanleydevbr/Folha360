using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using Folha360.Processamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Processamento.Infrastructure.Repositories;

public class ItemFolhaRepository : IItemFolhaRepository
{
    private readonly ProcessamentoDbContext _db;

    public ItemFolhaRepository(ProcessamentoDbContext db) => _db = db;

    public async Task<ItemFolha?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ItensFolha.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IEnumerable<ItemFolha>> GetByProcessamentoAsync(
        Guid processamentoId, CancellationToken ct = default)
        => await _db.ItensFolha
            .Where(i => i.ProcessamentoId == processamentoId)
            .OrderBy(i => i.Fase)
            .ThenBy(i => i.Ordem)
            .ToListAsync(ct);

    public async Task<IEnumerable<ItemFolha>> GetByFuncionarioAsync(
        Guid processamentoId, Guid funcionarioId, CancellationToken ct = default)
        => await _db.ItensFolha
            .Where(i => i.ProcessamentoId == processamentoId && i.FuncionarioId == funcionarioId)
            .OrderBy(i => i.Fase)
            .ThenBy(i => i.Ordem)
            .ToListAsync(ct);

    public async Task<IEnumerable<ItemFolha>> GetHistoricoMediasAsync(
        Guid funcionarioId, Guid rubricaId, int meses, DateOnly antesDe, CancellationToken ct = default)
    {
        var dataLimite = antesDe.AddMonths(-meses);
        return await _db.ItensFolha
            .Where(i =>
                i.FuncionarioId == funcionarioId &&
                i.RubricaId == rubricaId &&
                i.DataCalculo >= dataLimite.ToDateTime(TimeOnly.MinValue) &&
                i.DataCalculo < antesDe.ToDateTime(TimeOnly.MinValue))
            .OrderByDescending(i => i.DataCalculo)
            .ToListAsync(ct);
    }

    public async Task AddAsync(ItemFolha item, CancellationToken ct = default)
    {
        _db.ItensFolha.Add(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddBatchAsync(IEnumerable<ItemFolha> itens, CancellationToken ct = default)
    {
        _db.ItensFolha.AddRange(itens);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default)
    {
        var itens = await _db.ItensFolha
            .Where(i => i.ProcessamentoId == processamentoId)
            .ToListAsync(ct);

        foreach (var item in itens)
        {
            item.DeletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }
}
