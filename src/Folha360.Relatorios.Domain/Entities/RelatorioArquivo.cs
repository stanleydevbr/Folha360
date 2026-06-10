namespace Folha360.Relatorios.Domain.Entities;

public class RelatorioArquivo
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; private set; }
    public TipoRelatorio TipoRelatorio { get; private set; }
    public string Periodo { get; private set; } = string.Empty;
    public FormatoExportacao Formato { get; private set; }
    public string Bucket { get; private set; } = string.Empty;
    public string Chave { get; private set; } = string.Empty;
    public long TamanhoBytes { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private RelatorioArquivo()
    {
    }

    public static RelatorioArquivo Registrar(
        Guid empresaId,
        TipoRelatorio tipoRelatorio,
        string periodo,
        FormatoExportacao formato,
        string bucket,
        string chave,
        long tamanhoBytes)
    {
        return new RelatorioArquivo
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            TipoRelatorio = tipoRelatorio,
            Periodo = periodo,
            Formato = formato,
            Bucket = bucket,
            Chave = chave,
            TamanhoBytes = tamanhoBytes,
            CriadoEm = DateTime.UtcNow,
        };
    }

    public void Invalidar()
    {
        DeletedAt = DateTime.UtcNow;
    }
}
