using Folha360.Fiscais.Domain;

namespace Folha360.Fiscais.Application.Services;

/// <summary>
/// Gera PDF de guia (GPS/DARF/GRF) com código de barras Febraban
/// usando QuestPDF + BarcodeStandard.
/// </summary>
public interface IGeradorGuiaPdfService
{
    /// <summary>
    /// Gera o PDF da guia de recolhimento e retorna o stream.
    /// </summary>
    Task<Stream> GerarAsync(
        Guid empresaId,
        DateOnly periodo,
        Tributo tributo,
        TipoGuia tipoGuia,
        decimal valor,
        DateOnly dataVencimento,
        string codigoReceita,
        CancellationToken ct = default);
}
