using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaProcessoRepository
{
    Task<RubricaProcesso?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<RubricaProcesso>> GetByRubricaIdAsync(Guid rubricaId, CancellationToken ct = default);
    Task<IEnumerable<RubricaProcesso>> GetByProcessoIdAsync(Guid processoAdministrativoId, CancellationToken ct = default);
    Task AddAsync(RubricaProcesso entity, CancellationToken ct = default);
    Task RemoveAsync(Guid id, CancellationToken ct = default);
}
