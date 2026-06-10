using Folha360.Processamento.Application.Services;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Entities;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Folha360.Processamento.Infrastructure.Services;

public class PdfGeradorService : IPdfGeradorService
{
    private readonly ILogger<PdfGeradorService> _logger;

    public PdfGeradorService(ILogger<PdfGeradorService> logger)
    {
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GerarHolerite(
        ProcessamentoFolha processamento,
        IEnumerable<ItemFolha> itens,
        string nomeFuncionario,
        string cpfFuncionario,
        string nomeEmpresa,
        string cnpjEmpresa,
        string? dadosBancarios = null)
    {
        _logger.LogInformation(
            "Gerando holerite para {FuncionarioNome} no processamento {ProcessamentoId}",
            nomeFuncionario, processamento.Id);

        var itensList = itens.ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text(text =>
                {
                    text.Span(nomeEmpresa).FontSize(14).Bold();
                    text.EmptyLine();
                    text.Span($"CNPJ: {cnpjEmpresa}").FontSize(9);
                    text.EmptyLine();
                    text.EmptyLine();
                    text.AlignCenter();
                    text.Span("HOLERITE DE PAGAMENTO").FontSize(12).Bold();
                    text.EmptyLine();
                    text.EmptyLine();
                    text.Span($"Período: {processamento.Periodo:MM/yyyy}").FontSize(10);
                    text.EmptyLine();
                    text.Span($"Tipo: {processamento.TipoCalculo}").FontSize(10);
                    text.EmptyLine();
                    text.Span($"Funcionário: {nomeFuncionario}").FontSize(10);
                    text.EmptyLine();
                    text.Span($"CPF: {cpfFuncionario}").FontSize(10);
                });

                page.Content().PaddingVertical(10).Text(text =>
                {
                    var vencimentos = itensList.Where(i => i.Fase == FaseProcessamento.Vencimentos).ToList();
                    if (vencimentos.Count > 0)
                    {
                        text.Span("VENCIMENTOS").FontSize(11).Bold().Underline();
                        text.EmptyLine();
                        foreach (var item in vencimentos.OrderBy(i => i.Ordem))
                        {
                            text.Span($"  Rubrica: {item.RubricaId} — Valor: R$ {item.Valor:N2}");
                            text.EmptyLine();
                        }

                        text.EmptyLine();
                    }

                    var descontos = itensList.Where(i => i.Fase == FaseProcessamento.Descontos).ToList();
                    if (descontos.Count > 0)
                    {
                        text.Span("DESCONTOS").FontSize(11).Bold().Underline();
                        text.EmptyLine();
                        foreach (var item in descontos.OrderBy(i => i.Ordem))
                        {
                            text.Span($"  Rubrica: {item.RubricaId} — Valor: R$ {item.Valor:N2}");
                            text.EmptyLine();
                        }

                        text.EmptyLine();
                    }

                    text.Span("TOTAIS").FontSize(11).Bold().Underline();
                    text.EmptyLine();
                    text.EmptyLine();
                    text.Span($"  Total Vencimentos: R$ {processamento.TotalVencimentos:N2}");
                    text.EmptyLine();
                    text.Span($"  Total Descontos:   R$ {processamento.TotalDescontos:N2}");
                    text.EmptyLine();
                    text.Span($"  Líquido:           R$ {processamento.TotalLiquido:N2}").Bold();
                });

                if (!string.IsNullOrWhiteSpace(dadosBancarios))
                {
                    page.Footer().Text(text =>
                    {
                        text.Span("DADOS BANCÁRIOS").FontSize(10).Bold();
                        text.EmptyLine();
                        text.Span(dadosBancarios).FontSize(9);
                    });
                }
            });
        });

        return document.GeneratePdf();
    }
}
