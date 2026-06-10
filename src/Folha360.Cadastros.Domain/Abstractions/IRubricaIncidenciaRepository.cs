using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaIncidenciaRepository
{
    Task<IEnumerable<RubricaIncidencia>> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default);
    Task<RubricaIncidencia?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid rubricaId, string tipoIncidencia, CancellationToken ct = default);
    Task AddAsync(RubricaIncidencia incidencia, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
