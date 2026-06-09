using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Handlers;

public class EnviarExportacaoSftpHandler : IRequestHandler<EnviarExportacaoSftpCommand, Result<bool>>
{
    private readonly ILogger<EnviarExportacaoSftpHandler> _logger;

    public EnviarExportacaoSftpHandler(ILogger<EnviarExportacaoSftpHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> Handle(EnviarExportacaoSftpCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Envio SFTP solicitado para empresa {EmpresaId} período {Periodo}", request.EmpresaId, request.Periodo);
        return Task.FromResult(Result<bool>.Success(true));
    }
}
