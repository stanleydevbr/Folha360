using Folha360.Fiscais.Application.Services;
using Folha360.Fiscais.Domain;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Infrastructure.Services;

/// <summary>
/// Stub do gerador de PDF de guias. Será implementado com QuestPDF + BarcodeStandard.
/// </summary>
public class GeradorGuiaPdfService : IGeradorGuiaPdfService
{
    private readonly ILogger<GeradorGuiaPdfService> _logger;

    public GeradorGuiaPdfService(ILogger<GeradorGuiaPdfService> logger)
    {
        _logger = logger;
    }

    public Task<Stream> GerarAsync(
        Guid empresaId,
        DateOnly periodo,
        Tributo tributo,
        TipoGuia tipoGuia,
        decimal valor,
        DateOnly dataVencimento,
        string codigoReceita,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[STUB] Gerando PDF de guia {TipoGuia} - {Tributo} para empresa {EmpresaId} período {Periodo}",
            tipoGuia, tributo, empresaId, periodo);

        // Retorna stream vazio como placeholder (PDF será implementado com QuestPDF + BarcodeStandard)
        return Task.FromResult<Stream>(new MemoryStream());
    }
}
