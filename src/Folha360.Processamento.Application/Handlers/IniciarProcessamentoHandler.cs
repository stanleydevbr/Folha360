using Folha360.Application;
using Folha360.Domain.Abstractions;
using Folha360.Processamento.Application.Commands;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Handlers;

public class IniciarProcessamentoHandler : IRequestHandler<IniciarProcessamentoCommand, Result<ProcessamentoResponse>>
{
    private readonly IProcessamentoRepository _repo;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<IniciarProcessamentoHandler> _logger;

    public IniciarProcessamentoHandler(
        IProcessamentoRepository repo,
        IMessageBus messageBus,
        ILogger<IniciarProcessamentoHandler> logger)
    {
        _repo = repo;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<ProcessamentoResponse>> Handle(
        IniciarProcessamentoCommand cmd, CancellationToken ct)
    {
        if (!DateOnly.TryParseExact(cmd.Periodo, "yyyy-MM", out var periodo))
            return Result<ProcessamentoResponse>.Failure("VALIDACAO", "Período deve estar no formato YYYY-MM.");

        if (!Enum.TryParse<TipoCalculo>(cmd.TipoCalculo, true, out var tipoCalculo))
            return Result<ProcessamentoResponse>.Failure("VALIDACAO", $"Tipo de cálculo inválido: {cmd.TipoCalculo}.");

        // Verificar idempotência
        var existente = await _repo.GetByEmpresaPeriodoAsync(cmd.EmpresaId, periodo, tipoCalculo, 1, ct);
        if (existente is not null && existente.Status != StatusProcessamento.Cancelado && existente.Status != StatusProcessamento.Falho)
        {
            return Result<ProcessamentoResponse>.Failure(
                "CONFLITO",
                $"Já existe um processamento ativo para este período. Id: {existente.Id}");
        }

        var processamento = new ProcessamentoFolha(cmd.EmpresaId, periodo, tipoCalculo);
        await _repo.AddAsync(processamento, ct);

        _logger.LogInformation(
            "Processamento iniciado: {ProcessamentoId} para empresa {EmpresaId}, período {Periodo}",
            processamento.Id, cmd.EmpresaId, cmd.Periodo);

        // Publicar comando interno para processamento assíncrono
        await _messageBus.PublishAsync(
            new ProcessarFolhaInternalCommand(processamento.Id),
            "folha360.processamento",
            "ProcessarFolha",
            ct);

        return Result<ProcessamentoResponse>.Success(MapResponse(processamento));
    }

    private static ProcessamentoResponse MapResponse(ProcessamentoFolha p) => new(
        p.Id,
        p.EmpresaId,
        $"{p.Periodo:yyyy-MM}",
        p.TipoCalculo.ToString(),
        p.Status.ToString(),
        p.Versao,
        p.TotalFuncionarios,
        p.FuncionariosProcessados,
        p.FuncionariosComErro,
        p.TotalVencimentos,
        p.TotalDescontos,
        p.TotalLiquido,
        p.DataInicio,
        p.DataFim,
        p.Erro);
}

public record ProcessarFolhaInternalCommand(Guid ProcessamentoId);
