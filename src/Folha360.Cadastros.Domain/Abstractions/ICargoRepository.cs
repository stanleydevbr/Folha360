using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface ICargoRepository
{
    Task<Cargo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Cargo>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Cargo> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? nome = null,
        CancellationToken ct = default);
    Task AddAsync(Cargo cargo, CancellationToken ct = default);
    Task UpdateAsync(Cargo cargo, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasFuncionariosVinculadosAsync(Guid id, CancellationToken ct = default);
}
