using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaComposicaoRepository
{
    Task<IEnumerable<RubricaComposicao>> GetByPrincipalAsync(Guid rubricaPrincipalId, CancellationToken ct = default);
    Task<RubricaComposicao?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsCaminhoAsync(Guid origemId, Guid destinoId, CancellationToken ct = default);
    Task AddAsync(RubricaComposicao composicao, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
