using Folha360.Fiscais.Domain.Entities;

namespace Folha360.Fiscais.Domain.Abstractions;

public interface IApuracaoFiscalRepository
{
    Task<ApuracaoFiscal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ApuracaoFiscal>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default);
    Task<IReadOnlyList<ApuracaoFiscal>> GetByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default);
    Task AddAsync(ApuracaoFiscal apuracao, CancellationToken ct = default);
    Task UpdateAsync(ApuracaoFiscal apuracao, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid empresaId, DateOnly periodo, Tributo tributo, Guid processamentoId, CancellationToken ct = default);
}
