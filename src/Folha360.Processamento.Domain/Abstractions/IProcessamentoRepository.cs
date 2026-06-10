using Folha360.Processamento.Domain.Entities;

namespace Folha360.Processamento.Domain.Abstractions;

public interface IProcessamentoRepository
{
    Task<ProcessamentoFolha?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProcessamentoFolha?> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, TipoCalculo tipoCalculo, int versao, CancellationToken ct = default);
    Task<IEnumerable<ProcessamentoFolha>> GetHistoricoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default);
    Task<(IEnumerable<ProcessamentoFolha> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? empresaId = null, DateOnly? periodo = null,
        StatusProcessamento? status = null, TipoCalculo? tipoCalculo = null,
        CancellationToken ct = default);
    Task AddAsync(ProcessamentoFolha processamento, CancellationToken ct = default);
    Task UpdateAsync(ProcessamentoFolha processamento, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
