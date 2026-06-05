using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface ISindicatoRepository
{
    Task<Sindicato?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Sindicato?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<Sindicato>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Sindicato> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? nome = null,
        CancellationToken ct = default);
    Task AddAsync(Sindicato sindicato, CancellationToken ct = default);
    Task UpdateAsync(Sindicato sindicato, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
