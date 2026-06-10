namespace Folha360.Relatorios.Application.Services;

public interface IRelatorioStorageService
{
    Task<string> ArmazenarAsync(string bucket, string chave, Stream conteudo, string contentType, CancellationToken ct);
    Task<Stream> RecuperarAsync(string bucket, string chave, CancellationToken ct);
    Task<bool> ExisteAsync(string bucket, string chave, CancellationToken ct);
    Task<string> GerarUrlAssinadaAsync(string bucket, string chave, TimeSpan expiracao, CancellationToken ct);
}
