using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IDependenteRepository
{
    Task<Dependente?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Dependente>> GetByFuncionarioIdAsync(Guid funcionarioId, CancellationToken ct = default);
    Task<(IEnumerable<Dependente> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? funcionarioId = null, string? tipo = null,
        CancellationToken ct = default);
    Task AddAsync(Dependente dependente, CancellationToken ct = default);
    Task UpdateAsync(Dependente dependente, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
