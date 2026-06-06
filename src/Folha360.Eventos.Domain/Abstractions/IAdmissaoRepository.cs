using Folha360.Eventos.Domain.Entities;

namespace Folha360.Eventos.Domain.Abstractions;

public interface IAdmissaoRepository
{
    Task<Admissao?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Admissao>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Admissao> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? funcionarioId = null, CancellationToken ct = default);
    Task AddAsync(Admissao entity, CancellationToken ct = default);
    Task UpdateAsync(Admissao entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
