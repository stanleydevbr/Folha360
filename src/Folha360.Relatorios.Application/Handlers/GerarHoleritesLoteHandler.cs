using Folha360.Application;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Handlers;

public class GerarHoleritesLoteHandler : IRequestHandler<GerarHoleritesLoteCommand, Result<Guid>>
{
    private readonly IRelatorioRepository _relatorioRepository;
    private readonly IRelatorioPdfService _pdfService;
    private readonly IRelatorioStorageService _storageService;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly ILogger<GerarHoleritesLoteHandler> _logger;

    public GerarHoleritesLoteHandler(
        IRelatorioRepository relatorioRepository,
        IRelatorioPdfService pdfService,
        IRelatorioStorageService storageService,
        IAgendamentoRepository agendamentoRepository,
        ILogger<GerarHoleritesLoteHandler> logger)
    {
        _relatorioRepository = relatorioRepository;
        _pdfService = pdfService;
        _storageService = storageService;
        _agendamentoRepository = agendamentoRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(GerarHoleritesLoteCommand request, CancellationToken ct)
    {
        var loteId = Guid.NewGuid();
        _logger.LogInformation("Iniciando lote de holerites {LoteId} para empresa {EmpresaId}", loteId, request.EmpresaId);

        // Fire and forget — processamento assíncrono
        _ = Task.Run(async () =>
        {
            try
            {
                var itens = await _relatorioRepository.ObterItensFolhaAsync(request.EmpresaId, request.Periodo, ct);

                var funcionariosIds = itens
                    .Select(i => i.FuncionarioId)
                    .Distinct()
                    .Where(fid => request.FuncionarioIds is null || request.FuncionarioIds.Count == 0 || request.FuncionarioIds.Contains(fid))
                    .ToList();

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    CancellationToken = ct,
                };

                await Parallel.ForEachAsync(funcionariosIds, parallelOptions, async (funcionarioId, innerCt) =>
                {
                    var chave = $"{request.EmpresaId}/{request.Periodo}/{funcionarioId}.pdf";

                    // Idempotência: não regenerar se já existe
                    if (await _storageService.ExisteAsync("folha360-holerites", chave, innerCt))
                    {
                        _logger.LogDebug("Holerite já existe: {Chave}", chave);
                        return;
                    }

                    var funcItens = itens.Where(i => i.FuncionarioId == funcionarioId).ToList();
                    var dto = MapearHoleriteDto(request.EmpresaId, funcionarioId, request.Periodo, funcItens);

                    var pdfStream = await _pdfService.GerarHoleritePdfAsync(dto, innerCt);
                    await _storageService.ArmazenarAsync("folha360-holerites", chave, pdfStream, "application/pdf", innerCt);

                    var arquivo = Folha360.Relatorios.Domain.Entities.RelatorioArquivo.Registrar(
                        request.EmpresaId, Folha360.Relatorios.Domain.Enums.TipoRelatorio.Holerite,
                        request.Periodo, Folha360.Relatorios.Domain.Enums.FormatoExportacao.Pdf,
                        "folha360-holerites", chave, pdfStream.Length);

                    await _agendamentoRepository.AdicionarArquivoAsync(arquivo, innerCt);
                });

                _logger.LogInformation("Lote de holerites {LoteId} concluído", loteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no lote de holerites {LoteId}", loteId);
            }
        }, ct);

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
