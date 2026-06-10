namespace Folha360.Relatorios.Application.Services;

public interface IRelatorioExportService
{
    Task<Stream> ExportarCsvAsync<T>(IReadOnlyList<T> dados, CancellationToken ct);
    Task<Stream> ExportarCsvDirfAsync(IReadOnlyList<DirfDto> dados, CancellationToken ct);
    Task<Stream> ExportarCsvRaisAsync(IReadOnlyList<RaisDto> dados, CancellationToken ct);
    Task<Stream> ExportarXmlAsync<T>(IReadOnlyList<T> dados, string rootElementName, CancellationToken ct);
}
