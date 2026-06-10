using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaTabelaProgressivaRepository
{
    Task<IEnumerable<RubricaTabelaProgressiva>> GetByAnoVigenciaAsync(Guid rubricaId, int ano, CancellationToken ct = default);
    Task<IEnumerable<RubricaTabelaProgressiva>> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default);
    Task<RubricaTabelaProgressiva?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasSobreposicaoAsync(Guid rubricaId, int anoVigencia, decimal faixaDe, decimal? faixaAte, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(RubricaTabelaProgressiva faixa, CancellationToken ct = default);
    Task UpdateAsync(RubricaTabelaProgressiva faixa, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
