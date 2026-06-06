using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IFuncionarioRepository
{
    Task<Funcionario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Funcionario?> GetByCpfHashAsync(string cpfHash, CancellationToken ct = default);
    Task<IEnumerable<Funcionario>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Funcionario> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? status = null, Guid? cargoId = null,
        Guid? lotacaoId = null, string? nome = null,
        CancellationToken ct = default);
    Task AddAsync(Funcionario funcionario, CancellationToken ct = default);
    Task UpdateAsync(Funcionario funcionario, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
