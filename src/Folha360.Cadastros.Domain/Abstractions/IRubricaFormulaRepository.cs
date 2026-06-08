using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaFormulaRepository
{
    Task<RubricaFormula?> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default);
    Task AddAsync(RubricaFormula formula, CancellationToken ct = default);
    Task UpdateAsync(RubricaFormula formula, CancellationToken ct = default);
}
