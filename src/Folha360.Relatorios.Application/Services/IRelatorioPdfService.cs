namespace Folha360.Relatorios.Application.Services;

public interface IRelatorioPdfService
{
    Task<Stream> GerarHoleritePdfAsync(HoleriteDto dados, CancellationToken ct);
    Task<Stream> GerarFolhaAnaliticaPdfAsync(FolhaAnaliticaDto dados, CancellationToken ct);
    Task<Stream> GerarFolhaSinteticaPdfAsync(FolhaSinteticaDto dados, CancellationToken ct);
    Task<Stream> GerarResumoMensalPdfAsync(ResumoMensalDto dados, CancellationToken ct);
    Task<Stream> GerarResumoAnualPdfAsync(ResumoAnualDto dados, CancellationToken ct);
    Task<Stream> GerarRelatorioPdfAsync(RelatorioDto dados, CancellationToken ct);
}
