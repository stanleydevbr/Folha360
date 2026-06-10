using Folha360.Processamento.Domain.Entities;

namespace Folha360.Processamento.Domain.Abstractions;

public interface IHoleriteRepository
{
    Task<Holerite?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Holerite>> GetByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default);
    Task<Holerite?> GetByFuncionarioAsync(Guid processamentoId, Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(Holerite holerite, CancellationToken ct = default);
    Task AddBatchAsync(IEnumerable<Holerite> holerites, CancellationToken ct = default);
}
