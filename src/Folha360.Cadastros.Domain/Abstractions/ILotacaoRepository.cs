using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface ILotacaoRepository
{
    Task<Lotacao?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Lotacao?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<Lotacao>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Lotacao> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? codigo = null,
        CancellationToken ct = default);
    Task AddAsync(Lotacao lotacao, CancellationToken ct = default);
    Task UpdateAsync(Lotacao lotacao, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasFuncionariosVinculadosAsync(Guid id, CancellationToken ct = default);
}
