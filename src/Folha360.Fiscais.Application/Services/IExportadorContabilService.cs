using Folha360.Fiscais.Domain;

namespace Folha360.Fiscais.Application.Services;

/// <summary>
/// Gera arquivos CSV genérico e SPED ECD/ECF para exportação contábil.
/// </summary>
public interface IExportadorContabilService
{
    /// <summary>
    /// Exporta lançamentos contábeis no formato especificado.
    /// </summary>
    Task<Stream> ExportarAsync(
        Guid empresaId,
        DateOnly periodo,
        FormatoExportacao formato,
        IEnumerable<Domain.Entities.LancamentoContabil> lancamentos,
        CancellationToken ct = default);
}
