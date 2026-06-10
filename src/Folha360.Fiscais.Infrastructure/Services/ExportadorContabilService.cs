using Folha360.Fiscais.Application.Services;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Infrastructure.Services;

/// <summary>
/// Stub do exportador contábil. Será implementado com CsvHelper + layout SPED.
/// </summary>
public class ExportadorContabilService : IExportadorContabilService
{
    private readonly ILogger<ExportadorContabilService> _logger;

    public ExportadorContabilService(ILogger<ExportadorContabilService> logger)
    {
        _logger = logger;
    }

    public Task<Stream> ExportarAsync(
        Guid empresaId,
        DateOnly periodo,
        FormatoExportacao formato,
        IEnumerable<LancamentoContabil> lancamentos,
        CancellationToken ct = default)
    {
        var lancamentosList = lancamentos.ToList();
        _logger.LogInformation(
            "[STUB] Exportando {Count} lançamentos no formato {Formato} para empresa {EmpresaId} período {Periodo}",
            lancamentosList.Count, formato, empresaId, periodo);

        // Retorna stream vazio como placeholder (CSV será implementado com CsvHelper, SPED com layout oficial)
        return Task.FromResult<Stream>(new MemoryStream());
    }
}
