using Folha360.Application;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Handlers;

public class GerarHoleritesLoteHandler : IRequestHandler<GerarHoleritesLoteCommand, Result<Guid>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<GerarHoleritesLoteHandler> _logger;

    public GerarHoleritesLoteHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<GerarHoleritesLoteHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(GerarHoleritesLoteCommand request, CancellationToken ct)
    {
        var loteId = Guid.NewGuid();
        _logger.LogInformation("Iniciando lote de holerites {LoteId} para empresa {EmpresaId}", loteId, request.EmpresaId);

        // Processamento assíncrono com escopo próprio para evitar ObjectDisposedException
        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var relatorioRepository = scope.ServiceProvider.GetRequiredService<IRelatorioRepository>();
            var pdfService = scope.ServiceProvider.GetRequiredService<IRelatorioPdfService>();
            var storageService = scope.ServiceProvider.GetRequiredService<IRelatorioStorageService>();
            var agendamentoRepository = scope.ServiceProvider.GetRequiredService<IAgendamentoRepository>();
            var redisCache = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

            try
            {
                var itens = await relatorioRepository.ObterItensFolhaAsync(request.EmpresaId, request.Periodo, CancellationToken.None);

                var funcionariosIds = itens
                    .Select(i => i.FuncionarioId)
                    .Distinct()
                    .Where(fid => request.FuncionarioIds is null || request.FuncionarioIds.Count == 0 || request.FuncionarioIds.Contains(fid))
                    .ToList();

                var total = funcionariosIds.Count;
                var concluidos = 0;

                // Update initial status in cache
                await redisCache.ArmazenarAsync($"lote:{loteId}", new LoteStatusDto
                {
                    LoteId = loteId,
                    Status = "em_andamento",
                    ProgressoPercentual = 0,
                    EstimativaSegundos = total / 10,
                }, TimeSpan.FromHours(1), CancellationToken.None);

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    CancellationToken = CancellationToken.None,
                };

                var erros = new List<string>();

                await Parallel.ForEachAsync(funcionariosIds, parallelOptions, async (funcionarioId, innerCt) =>
                {
                    var chave = $"{request.EmpresaId}/{request.Periodo}/{funcionarioId}.pdf";

                    // Idempotência: não regenerar se já existe
                    if (await storageService.ExisteAsync("folha360-holerites", chave, innerCt))
                    {
                        Interlocked.Increment(ref concluidos);
                        return;
                    }

                    try
                    {
                        var funcItens = itens.Where(i => i.FuncionarioId == funcionarioId).ToList();
                        var dto = MapearHoleriteDto(request.EmpresaId, funcionarioId, request.Periodo, funcItens);

                        var pdfStream = await pdfService.GerarHoleritePdfAsync(dto, innerCt);
                        await storageService.ArmazenarAsync("folha360-holerites", chave, pdfStream, "application/pdf", innerCt);

                        var arquivo = Folha360.Relatorios.Domain.Entities.RelatorioArquivo.Registrar(
                            request.EmpresaId, Folha360.Relatorios.Domain.Enums.TipoRelatorio.Holerite,
                            request.Periodo, Folha360.Relatorios.Domain.Enums.FormatoExportacao.Pdf,
                            "folha360-holerites", chave, pdfStream.Length);

                        await agendamentoRepository.AdicionarArquivoAsync(arquivo, innerCt);
                    }
                    catch (Exception ex)
                    {
                        lock (erros)
                        {
                            erros.Add($"Funcionario {funcionarioId}: {ex.Message}");
                        }
                    }

                    var progresso = Interlocked.Increment(ref concluidos);
                    await redisCache.ArmazenarAsync($"lote:{loteId}", new LoteStatusDto
                    {
                        LoteId = loteId,
                        Status = "em_andamento",
                        ProgressoPercentual = (int)((double)progresso / total * 100),
                        EstimativaSegundos = (total - progresso) / 10,
                        Erros = erros.Count > 0 ? erros.ToList() : null,
                    }, TimeSpan.FromHours(1), CancellationToken.None);
                });

                // Final status
                await redisCache.ArmazenarAsync($"lote:{loteId}", new LoteStatusDto
                {
                    LoteId = loteId,
                    Status = erros.Count > 0 ? "concluido_com_erros" : "concluido",
                    ProgressoPercentual = 100,
                    EstimativaSegundos = 0,
                    Erros = erros.Count > 0 ? erros : null,
                }, TimeSpan.FromHours(1), CancellationToken.None);

                _logger.LogInformation("Lote de holerites {LoteId} concluído: {Concluidos}/{Total}", loteId, concluidos, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no lote de holerites {LoteId}", loteId);

                await redisCache.ArmazenarAsync($"lote:{loteId}", new LoteStatusDto
                {
                    LoteId = loteId,
                    Status = "falha",
                    ProgressoPercentual = 0,
                    EstimativaSegundos = 0,
                    Erros = new List<string> { ex.Message },
                }, TimeSpan.FromHours(1), CancellationToken.None);
            }
        }, CancellationToken.None);

        return Result<Guid>.Success(loteId);
    }

    private static HoleriteDto MapearHoleriteDto(Guid empresaId, Guid funcionarioId, string periodo, List<Folha360.Relatorios.Domain.Entities.ItemFolhaView> itens)
    {
        var primeiro = itens.First();
        return new HoleriteDto
        {
            EmpresaId = empresaId,
            NomeEmpresa = "Empresa",
            CnpjEmpresa = "00.000.000/0001-00",
            FuncionarioId = funcionarioId,
            NomeFuncionario = primeiro.NomeFuncionario,
            Cargo = primeiro.NomeDepartamento,
            Periodo = periodo,
            Vencimentos = itens.Where(i => i.Natureza == "VENCIMENTO")
                .Select(i => new RubricaItemDto { Codigo = i.CodigoRubrica, Nome = i.NomeRubrica, Valor = i.Valor }).ToList(),
            Descontos = itens.Where(i => i.Natureza == "DESCONTO")
                .Select(i => new RubricaItemDto { Codigo = i.CodigoRubrica, Nome = i.NomeRubrica, Valor = Math.Abs(i.Valor) }).ToList(),
            TotalVencimentos = itens.Where(i => i.Natureza == "VENCIMENTO").Sum(i => i.Valor),
            TotalDescontos = Math.Abs(itens.Where(i => i.Natureza == "DESCONTO").Sum(i => i.Valor)),
            Liquido = itens.Sum(i => i.Natureza == "VENCIMENTO" ? i.Valor : -i.Valor),
        };
    }
}
