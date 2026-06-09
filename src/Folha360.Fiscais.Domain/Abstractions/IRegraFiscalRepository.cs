using Folha360.Fiscais.Domain.Entities;

namespace Folha360.Fiscais.Domain.Abstractions;

public interface IRegraFiscalRepository
{
    Task<RegraFiscal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RegraFiscal?> GetVigenteAsync(Tributo tributo, DateOnly data, CancellationToken ct = default);
    Task<IReadOnlyList<RegraFiscal>> GetByTributoAsync(Tributo tributo, CancellationToken ct = default);
    Task<IReadOnlyList<RegraFiscal>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(RegraFiscal regra, CancellationToken ct = default);
    Task UpdateAsync(RegraFiscal regra, CancellationToken ct = default);
    Task<bool> ExistsAsync(Tributo tributo, int versao, CancellationToken ct = default);
}
