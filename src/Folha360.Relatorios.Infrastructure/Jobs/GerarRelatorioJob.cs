using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Folha360.Relatorios.Infrastructure.Jobs;

public class GerarRelatorioJob : IJob
{
    private readonly IRelatorioRepository _relatorioRepository;
    private readonly IRelatorioPdfService _pdfService;
    private readonly IRelatorioExportService _exportService;
    private readonly IRelatorioStorageService _storageService;
    private readonly IRelatorioEmailService _emailService;
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly ILogger<GerarRelatorioJob> _logger;

    public GerarRelatorioJob(
        IRelatorioRepository relatorioRepository,
        IRelatorioPdfService pdfService,
        IRelatorioExportService exportService,
        IRelatorioStorageService storageService,
        IRelatorioEmailService emailService,
        IAgendamentoRepository agendamentoRepository,
        ILogger<GerarRelatorioJob> logger)
    {
        _relatorioRepository = relatorioRepository;
        _pdfService = pdfService;
        _exportService = exportService;
        _storageService = storageService;
        _emailService = emailService;
        _agendamentoRepository = agendamentoRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var agendamentoId = context.MergedJobDataMap.GetGuid("agendamento_id");
        var empresaId = context.MergedJobDataMap.GetGuid("empresa_id");
        var tipoRelatorioStr = context.MergedJobDataMap.GetString("tipo_relatorio") ?? "Holerite";
        var formatoStr = context.MergedJobDataMap.GetString("formato") ?? "Pdf";
        var destinatariosStr = context.MergedJobDataMap.GetString("destinatarios") ?? "[]";

        var tipoRelatorio = Enum.Parse<TipoRelatorio>(tipoRelatorioStr);
        var formato = Enum.Parse<FormatoExportacao>(formatoStr);
        var periodo = DateTime.Now.ToString("yyyy-MM");

        var execucao = RelatorioExecucao.Iniciar(agendamentoId);
        await _agendamentoRepository.AdicionarExecucaoAsync(execucao, context.CancellationToken);

        try
        {
            _logger.LogInformation("Executando job de relatório: {Tipo} para empresa {EmpresaId}", tipoRelatorio, empresaId);

            // Generate report
            Stream? relatorioStream = null;
            string nomeArquivo = $"{tipoRelatorio}_{periodo}.{(formato == FormatoExportacao.Pdf ? "pdf" : formato.ToString().ToLower())}";
            string bucket = tipoRelatorio == TipoRelatorio.Holerite ? "folha360-holerites" : "folha360-relatorios";
            string chave = $"{empresaId}/{tipoRelatorio}/{periodo}/{nomeArquivo}";

            relatorioStream = await GerarRelatorioAsync(empresaId, periodo, tipoRelatorio, formato);

            if (relatorioStream is not null)
            {
                await _storageService.ArmazenarAsync(bucket, chave, relatorioStream, "application/octet-stream", context.CancellationToken);

                // Register file
                var arquivo = RelatorioArquivo.Registrar(empresaId, tipoRelatorio, periodo, formato, bucket, chave, relatorioStream.Length);
                await _agendamentoRepository.AdicionarArquivoAsync(arquivo, context.CancellationToken);

                // Send email to recipients
                var destinatarios = System.Text.Json.JsonSerializer.Deserialize<List<string>>(destinatariosStr) ?? new();
                if (destinatarios.Count > 0)
                {
                    relatorioStream.Position = 0;
                    var emailDestino = new Application.DTOs.EmailDestinoDto
                    {
                        EmpresaId = empresaId,
                        Destinatarios = destinatarios,
                        Assunto = $"Folha360 — {tipoRelatorio} — {periodo}",
                    };
                    await _emailService.EnviarAsync(emailDestino, relatorioStream, nomeArquivo, context.CancellationToken);
                }

                var urlAssinada = await _storageService.GerarUrlAssinadaAsync(bucket, chave, TimeSpan.FromDays(7), context.CancellationToken);
                execucao.Concluir(urlAssinada);
            }
            else
            {
                execucao.Falhar("Nenhum dado encontrado para gerar o relatório.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar job de relatório {AgendamentoId}", agendamentoId);
            execucao.Falhar(ex.ToString());
        }
    }

    private async Task<Stream?> GerarRelatorioAsync(Guid empresaId, string periodo, TipoRelatorio tipo, FormatoExportacao formato)
    {
        return tipo switch
        {
            TipoRelatorio.Holerite => await GerarHoleriteLoteAsync(empresaId, periodo, formato),
            TipoRelatorio.FolhaAnalitica => await GerarFolhaAnaliticaStreamAsync(empresaId, periodo, formato),
            TipoRelatorio.FolhaSintetica => await GerarFolhaSinteticaStreamAsync(empresaId, periodo, formato),
            TipoRelatorio.ResumoMensal => await GerarResumoMensalStreamAsync(empresaId, periodo, formato),
            _ => null,
        };
    }

    private async Task<Stream> GerarHoleriteLoteAsync(Guid empresaId, string periodo, FormatoExportacao formato)
    {
        // Simplified: generate a single report for the period
        var itens = await _relatorioRepository.ObterItensFolhaAsync(empresaId, periodo, CancellationToken.None);
        if (itens.Count == 0)
        {
            return new MemoryStream();
        }

        var dto = new Application.DTOs.FolhaAnaliticaDto
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            Funcionarios = itens.GroupBy(i => i.FuncionarioId).Select(g => new Application.DTOs.FuncionarioFolhaDto
            {
                FuncionarioId = g.Key,
                Nome = g.First().NomeFuncionario,
                Departamento = g.First().NomeDepartamento,
                Vencimentos = g.Where(i => i.Natureza == "VENCIMENTO").Select(i => new Application.DTOs.RubricaItemDto
                {
                    Codigo = i.CodigoRubrica,
                    Nome = i.NomeRubrica,
                    Valor = i.Valor,
                }).ToList(),
                Descontos = g.Where(i => i.Natureza == "DESCONTO").Select(i => new Application.DTOs.RubricaItemDto
                {
                    Codigo = i.CodigoRubrica,
                    Nome = i.NomeRubrica,
                    Valor = Math.Abs(i.Valor),
                }).ToList(),
                TotalVencimentos = g.Where(i => i.Natureza == "VENCIMENTO").Sum(i => i.Valor),
                TotalDescontos = Math.Abs(g.Where(i => i.Natureza == "DESCONTO").Sum(i => i.Valor)),
                Liquido = g.Sum(i => i.Natureza == "VENCIMENTO" ? i.Valor : -i.Valor),
            }).ToList(),
        };

        return await _pdfService.GerarFolhaAnaliticaPdfAsync(dto, CancellationToken.None);
    }

    private async Task<Stream> GerarFolhaAnaliticaStreamAsync(Guid empresaId, string periodo, FormatoExportacao formato)
    {
        var itens = await _relatorioRepository.ObterItensFolhaAsync(empresaId, periodo, CancellationToken.None);
        var dto = new Application.DTOs.FolhaAnaliticaDto
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            Funcionarios = itens.GroupBy(i => i.FuncionarioId).Select(g => new Application.DTOs.FuncionarioFolhaDto
            {
                FuncionarioId = g.Key,
                Nome = g.First().NomeFuncionario,
                Departamento = g.First().NomeDepartamento,
                TotalVencimentos = g.Where(i => i.Natureza == "VENCIMENTO").Sum(i => i.Valor),
                TotalDescontos = Math.Abs(g.Where(i => i.Natureza == "DESCONTO").Sum(i => i.Valor)),
                Liquido = g.Sum(i => i.Natureza == "VENCIMENTO" ? i.Valor : -i.Valor),
            }).ToList(),
        };

        return formato switch
        {
            FormatoExportacao.Csv => await _exportService.ExportarCsvAsync(dto.Funcionarios, CancellationToken.None),
            FormatoExportacao.Xml => await _exportService.ExportarXmlAsync(dto.Funcionarios, "funcionarios", CancellationToken.None),
            _ => await _pdfService.GerarFolhaAnaliticaPdfAsync(dto, CancellationToken.None),
        };
    }

    private async Task<Stream> GerarFolhaSinteticaStreamAsync(Guid empresaId, string periodo, FormatoExportacao formato)
    {
        var totais = await _relatorioRepository.ObterFolhaSinteticaAsync(empresaId, periodo, CancellationToken.None);
        var dto = new Application.DTOs.FolhaSinteticaDto
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            TotaisPorRubrica = totais.Select(kvp =>
            {
                var parts = kvp.Key.Split('|');
                return new Application.DTOs.RubricaTotalDto
                {
                    Codigo = parts[0],
                    Nome = parts.Length > 1 ? parts[1] : string.Empty,
                    Natureza = parts.Length > 2 ? parts[2] : string.Empty,
                    Valor = kvp.Value,
                };
            }).ToList(),
        };

        return await _pdfService.GerarFolhaSinteticaPdfAsync(dto, CancellationToken.None);
    }

    private async Task<Stream> GerarResumoMensalStreamAsync(Guid empresaId, string periodo, FormatoExportacao formato)
    {
        var dados = await _relatorioRepository.ObterResumoMensalAsync(empresaId, periodo, CancellationToken.None);
        var dto = new Application.DTOs.ResumoMensalDto
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            TotalFuncionarios = Convert.ToInt32(dados["total_funcionarios"]),
            TotalVencimentos = Convert.ToDecimal(dados["total_vencimentos"]),
            TotalDescontos = Convert.ToDecimal(dados["total_descontos"]),
            TotalLiquido = Convert.ToDecimal(dados["total_liquido"]),
            TotalIrrf = Convert.ToDecimal(dados["total_irrf"]),
            TotalInss = Convert.ToDecimal(dados["total_inss"]),
            TotalFgts = Convert.ToDecimal(dados["total_fgts"]),
        };

        return formato switch
        {
            FormatoExportacao.Csv => await _exportService.ExportarCsvAsync(new[] { dto }, CancellationToken.None),
            _ => await _pdfService.GerarResumoMensalPdfAsync(dto, CancellationToken.None),
        };
    }
}
