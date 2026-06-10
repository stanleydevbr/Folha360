using Folha360.Application;
using Folha360.Domain.Abstractions;
using Folha360.Processamento.Application.Commands;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Handlers;

public class ReabrirProcessamentoHandler : IRequestHandler<ReabrirProcessamentoCommand, Result<ReaberturaStatusResponse>>
{
    private readonly IProcessamentoRepository _processamentoRepo;
    private readonly ICadeiaFechamentoRepository _cadeiaRepo;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ReabrirProcessamentoHandler> _logger;

    public ReabrirProcessamentoHandler(
        IProcessamentoRepository processamentoRepo,
        ICadeiaFechamentoRepository cadeiaRepo,
        IMessageBus messageBus,
        ILogger<ReabrirProcessamentoHandler> logger)
    {
        _processamentoRepo = processamentoRepo;
        _cadeiaRepo = cadeiaRepo;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<ReaberturaStatusResponse>> Handle(
        ReabrirProcessamentoCommand cmd, CancellationToken ct)
    {
        // R01: Validar motivo (mín. 20 caracteres)
        if (string.IsNullOrWhiteSpace(cmd.Motivo) || cmd.Motivo.Length < 20)
            return Result<ReaberturaStatusResponse>.Failure(
                "VALIDACAO", "Motivo deve ter no mínimo 20 caracteres.");

        var processamento = await _processamentoRepo.GetByIdAsync(cmd.ProcessamentoId, ct);
        if (processamento is null)
            return Result<ReaberturaStatusResponse>.Failure(
                "NAO_ENCONTRADO", "Processamento não encontrado.");

        // R09: Validar que não está Reaberta
        if (processamento.Status == StatusProcessamento.Reaberta)
            return Result<ReaberturaStatusResponse>.Failure(
                "CONFLITO", "Processamento já está reaberto.");

        // Verificar se está concluído
        if (processamento.Status != StatusProcessamento.Concluido)
            return Result<ReaberturaStatusResponse>.Failure(
                "VALIDACAO", "Apenas processamentos concluídos podem ser reabertos.");

        // R10: Validar prazo de 60 dias
        var diasDesdeFechamento = (DateTime.UtcNow - (processamento.DataFim ?? DateTime.UtcNow)).TotalDays;
        if (diasDesdeFechamento > 60)
        {
            if (cmd.Motivo.Length < 50)
                return Result<ReaberturaStatusResponse>.Failure(
                    "VALIDACAO", "Para reabertura após 60 dias, é necessária justificativa de força maior (mín. 50 caracteres).");
        }

        // Buscar cadeia de fechamento
        var cadeia = await _cadeiaRepo.GetByEmpresaPeriodoAsync(
            processamento.EmpresaId, processamento.Periodo, ct);

        // R07: Publicar FolhaReabertaEvent
        var evento = new FolhaReabertaEvent(
            processamento.EmpresaId,
            $"{processamento.Periodo:yyyy-MM}",
            processamento.Id,
            processamento.Versao,
            cmd.Motivo,
            cmd.Autor.ToString(),
            DateTime.UtcNow);

        await _messageBus.PublishAsync(evento, "folha360.processamento", "FolhaReaberta", ct);

        _logger.LogInformation(
            "Reabertura solicitada: Processamento {ProcessamentoId}, Motivo: {Motivo}",
            cmd.ProcessamentoId, cmd.Motivo);

        return Result<ReaberturaStatusResponse>.Success(new ReaberturaStatusResponse(
            Guid.NewGuid(),
            "ReversaoIniciada",
            cadeia?.Etapa.ToString() ?? "FolhaProcessada",
            DateTime.UtcNow,
            null));
    }
}
