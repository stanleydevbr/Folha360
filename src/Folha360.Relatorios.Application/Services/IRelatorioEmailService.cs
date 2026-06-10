namespace Folha360.Relatorios.Application.Services;

public interface IRelatorioEmailService
{
    Task EnviarAsync(EmailDestinoDto destino, Stream? anexo, string? nomeArquivo, CancellationToken ct);
}
