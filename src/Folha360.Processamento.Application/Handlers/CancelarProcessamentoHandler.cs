using Folha360.Application;
using Folha360.Processamento.Application.Commands;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Handlers;

public class CancelarProcessamentoHandler : IRequestHandler<CancelarProcessamentoCommand, Result<bool>>
{
    private readonly IProcessamentoRepository _processamentoRepo;
    private readonly IItemFolhaRepository _itemFolhaRepo;
    private readonly ILogger<CancelarProcessamentoHandler> _logger;

    public CancelarProcessamentoHandler(
        IProcessamentoRepository processamentoRepo,
        IItemFolhaRepository itemFolhaRepo,
        ILogger<CancelarProcessamentoHandler> logger)
    {
        _processamentoRepo = processamentoRepo;
        _itemFolhaRepo = itemFolhaRepo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        CancelarProcessamentoCommand cmd, CancellationToken ct)
    {
        var processamento = await _processamentoRepo.GetByIdAsync(cmd.ProcessamentoId, ct);
        if (processamento is null)
            return Result<bool>.Failure("NAO_ENCONTRADO", "Processamento não encontrado.");

        if (processamento.Status != StatusProcessamento.EmProcessamento)
            return Result<bool>.Failure("VALIDACAO", "Apenas processamentos em andamento podem ser cancelados.");

        processamento.Cancelar();
        await _processamentoRepo.UpdateAsync(processamento, ct);

        await _itemFolhaRepo.SoftDeleteByProcessamentoAsync(cmd.ProcessamentoId, ct);

        _logger.LogInformation("Processamento cancelado: {ProcessamentoId}", cmd.ProcessamentoId);

        return Result<bool>.Success(true);
    }
}
