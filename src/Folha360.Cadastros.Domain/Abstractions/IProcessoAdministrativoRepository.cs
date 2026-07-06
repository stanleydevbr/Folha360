using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IProcessoAdministrativoRepository
{
    Task<ProcessoAdministrativo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ProcessoAdministrativo>> GetByEmpresaIdAsync(Guid empresaId, CancellationToken ct = default);
    Task<(IEnumerable<ProcessoAdministrativo> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? tipo = null,
        CancellationToken ct = default);
    Task AddAsync(ProcessoAdministrativo entity, CancellationToken ct = default);
    Task UpdateAsync(ProcessoAdministrativo entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
