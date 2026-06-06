using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Domain.Abstractions;

public interface IDesligamentoRepository
{
    Task<Desligamento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Desligamento>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Desligamento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default);
    Task AddAsync(Desligamento entity, CancellationToken ct = default);
    Task UpdateAsync(Desligamento entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
