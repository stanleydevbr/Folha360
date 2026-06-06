using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Domain.Abstractions;

public interface IAlteracaoContratualRepository
{
    Task<AlteracaoContratual?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<AlteracaoContratual>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<AlteracaoContratual> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default);
    Task AddAsync(AlteracaoContratual entity, CancellationToken ct = default);
    Task UpdateAsync(AlteracaoContratual entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
