using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaHistoricoRepository
{
    Task<(IEnumerable<RubricaHistorico> Items, int TotalCount)> GetByRubricaPagedAsync(
        Guid rubricaId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(RubricaHistorico historico, CancellationToken ct = default);
}
