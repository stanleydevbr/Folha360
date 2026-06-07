using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Domain.Abstractions;

public interface IFeriasRepository
{
    Task<Ferias?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Ferias>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Ferias> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default);
    Task AddAsync(Ferias entity, CancellationToken ct = default);
    Task UpdateAsync(Ferias entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
