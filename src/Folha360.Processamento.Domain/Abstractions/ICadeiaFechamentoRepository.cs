using Folha360.Processamento.Domain.Entities;

namespace Folha360.Processamento.Domain.Abstractions;

public interface ICadeiaFechamentoRepository
{
    Task<CadeiaFechamento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CadeiaFechamento?> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default);
    Task AddAsync(CadeiaFechamento cadeia, CancellationToken ct = default);
    Task UpdateAsync(CadeiaFechamento cadeia, CancellationToken ct = default);
}
