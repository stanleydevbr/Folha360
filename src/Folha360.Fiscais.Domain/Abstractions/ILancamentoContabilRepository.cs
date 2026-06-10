using Folha360.Fiscais.Domain.Entities;

namespace Folha360.Fiscais.Domain.Abstractions;

public interface ILancamentoContabilRepository
{
    Task<LancamentoContabil?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LancamentoContabil>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default);
    Task AddAsync(LancamentoContabil lancamento, CancellationToken ct = default);
    Task UpdateAsync(LancamentoContabil lancamento, CancellationToken ct = default);
}
