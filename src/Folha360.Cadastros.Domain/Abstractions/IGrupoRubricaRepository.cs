using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IGrupoRubricaRepository
{
    Task<GrupoRubrica?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GrupoRubrica?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<GrupoRubrica>> GetAllByEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<(IEnumerable<GrupoRubrica> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? natureza = null,
        CancellationToken ct = default);
    Task<bool> HasRubricasVinculadasAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(GrupoRubrica grupo, CancellationToken ct = default);
    Task UpdateAsync(GrupoRubrica grupo, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
