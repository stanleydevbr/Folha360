namespace Folha360.Fiscais.Application.Services;

/// <summary>
/// Envia arquivos via SFTP usando SSH.NET.
/// </summary>
public interface ISftpService
{
    /// <summary>
    /// Envia um arquivo para o servidor SFTP configurado para a empresa.
    /// </summary>
    Task EnviarAsync(
        Guid empresaId,
        string nomeArquivo,
        Stream conteudo,
        CancellationToken ct = default);
}
