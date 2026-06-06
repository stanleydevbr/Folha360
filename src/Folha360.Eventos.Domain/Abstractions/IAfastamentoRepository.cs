using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Domain.Abstractions;

public interface IAfastamentoRepository
{
    Task<Afastamento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Afastamento>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Afastamento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default);
    Task AddAsync(Afastamento entity, CancellationToken ct = default);
    Task UpdateAsync(Afastamento entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
