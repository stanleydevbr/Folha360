using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Events;
using Folha360.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Handlers;

public class ReverterObrigacoesHandler : IRequestHandler<Commands.ReverterObrigacoesCommand, Result<bool>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;
    private readonly IGuiaRecolhimentoRepository _guiaRepo;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ReverterObrigacoesHandler> _logger;

    public ReverterObrigacoesHandler(
        IApuracaoFiscalRepository apuracaoRepo,
        IGuiaRecolhimentoRepository guiaRepo,
        IMessageBus messageBus,
        ILogger<ReverterObrigacoesHandler> logger)
    {
        _apuracaoRepo = apuracaoRepo;
        _guiaRepo = guiaRepo;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(Commands.ReverterObrigacoesCommand request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");

        var apuracoes = await _apuracaoRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);
        var guias = await _guiaRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        // Reverter todas as guias
        foreach (var guia in guias)
        {
            guia.Cancelar();
            await _guiaRepo.UpdateAsync(guia, ct);
        }

        // Soft delete nas apurações
        foreach (var apuracao in apuracoes)
        {
            apuracao.Reverter();
            await _apuracaoRepo.UpdateAsync(apuracao, ct);
        }

        await _messageBus.PublishAsync(
            new ObrigacoesRevertidasEvent(request.EmpresaId, request.Periodo, DateTime.UtcNow),
            "folha360.fiscais",
            "obrigacoes.revertidas",
            ct);

        _logger.LogInformation("Obrigações fiscais revertidas para empresa {EmpresaId} período {Periodo}", request.EmpresaId, request.Periodo);

        return Result<bool>.Success(true);
    }
}
