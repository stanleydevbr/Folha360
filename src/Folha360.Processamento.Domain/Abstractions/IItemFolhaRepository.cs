using Folha360.Processamento.Domain.Entities;

namespace Folha360.Processamento.Domain.Abstractions;

public interface IItemFolhaRepository
{
    Task<ItemFolha?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ItemFolha>> GetByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default);
    Task<IEnumerable<ItemFolha>> GetByFuncionarioAsync(Guid processamentoId, Guid funcionarioId, CancellationToken ct = default);
    Task<IEnumerable<ItemFolha>> GetHistoricoMediasAsync(Guid funcionarioId, Guid rubricaId, int meses, DateOnly antesDe, CancellationToken ct = default);
    Task AddAsync(ItemFolha item, CancellationToken ct = default);
    Task AddBatchAsync(IEnumerable<ItemFolha> itens, CancellationToken ct = default);
    Task SoftDeleteByProcessamentoAsync(Guid processamentoId, CancellationToken ct = default);
}
