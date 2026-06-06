using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IRubricaRepository
{
    Task<Rubrica?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Rubrica?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<Rubrica>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Rubrica> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? natureza = null, string? tipoEsocial = null,
        CancellationToken ct = default);
    Task AddAsync(Rubrica rubrica, CancellationToken ct = default);
    Task UpdateAsync(Rubrica rubrica, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
