using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Relatorios.Domain.Abstractions;

public interface IRelatorioRepository
{
    Task<IReadOnlyList<ItemFolhaView>> ObterItensFolhaAsync(Guid empresaId, string periodo, CancellationToken ct);
    Task<IReadOnlyList<DirfAnualView>> ObterDirfAnualAsync(Guid empresaId, int ano, CancellationToken ct);
    Task<IReadOnlyList<RaisAnualView>> ObterRaisAnualAsync(Guid empresaId, int ano, CancellationToken ct);
    Task<IReadOnlyList<ItemFolhaView>> ObterFolhaAnaliticaAsync(Guid empresaId, string periodo, Guid? departamentoId, string? tipoCalculo, CancellationToken ct);
    Task<Dictionary<string, decimal>> ObterFolhaSinteticaAsync(Guid empresaId, string periodo, CancellationToken ct);
    Task<Dictionary<string, object>> ObterResumoMensalAsync(Guid empresaId, string periodo, CancellationToken ct);
    Task<IReadOnlyList<Dictionary<string, object>>> ObterResumoAnualAsync(Guid empresaId, int ano, CancellationToken ct);
}
