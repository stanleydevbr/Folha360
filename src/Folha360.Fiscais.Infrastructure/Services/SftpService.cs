using Folha360.Fiscais.Application.Services;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Infrastructure.Services;

/// <summary>
/// Stub do serviço SFTP. Será implementado com SSH.NET.
/// </summary>
public class SftpService : ISftpService
{
    private readonly ILogger<SftpService> _logger;

    public SftpService(ILogger<SftpService> logger)
    {
        _logger = logger;
    }

    public Task EnviarAsync(
        Guid empresaId,
        string nomeArquivo,
        Stream conteudo,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[STUB] Enviando arquivo {NomeArquivo} via SFTP para empresa {EmpresaId}",
            nomeArquivo, empresaId);

        // Stub: não envia nada. Será implementado com SSH.NET.
        return Task.CompletedTask;
    }
}
