using Folha360.Application;
using Folha360.Processamento.Application.Commands;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Handlers;

public class ReprocessarFolhaHandler : IRequestHandler<ReprocessarFolhaCommand, Result<ProcessamentoResponse>>
{
    private readonly IProcessamentoRepository _repo;
    private readonly ILogger<ReprocessarFolhaHandler> _logger;

    public ReprocessarFolhaHandler(
        IProcessamentoRepository repo,
        ILogger<ReprocessarFolhaHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<ProcessamentoResponse>> Handle(
        ReprocessarFolhaCommand cmd, CancellationToken ct)
    {
        var original = await _repo.GetByIdAsync(cmd.ProcessamentoId, ct);
        if (original is null)
            return Result<ProcessamentoResponse>.Failure("NAO_ENCONTRADO", "Processamento original não encontrado.");

        if (original.Status != StatusProcessamento.Concluido && original.Status != StatusProcessamento.Reaberta)
            return Result<ProcessamentoResponse>.Failure(
                "VALIDACAO",
                "Apenas processamentos concluídos ou reabertos podem ser reprocessados.");

        // Soft delete do original
        await _repo.SoftDeleteAsync(original.Id, ct);

        // Criar novo com versão incrementada
        var novo = new ProcessamentoFolha(
            original.EmpresaId,
            original.Periodo,
            original.TipoCalculo,
            original.Versao + 1,
            original.Id);

        await _repo.AddAsync(novo, ct);

        _logger.LogInformation(
            "Reprocessamento iniciado: {NovoId} (original: {OriginalId}, versão: {Versao})",
            novo.Id, original.Id, novo.Versao);

        return Result<ProcessamentoResponse>.Success(MapResponse(novo));
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
