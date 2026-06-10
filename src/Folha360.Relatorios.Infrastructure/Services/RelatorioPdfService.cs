using System.Text;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Folha360.Relatorios.Infrastructure.Services;

public class RelatorioPdfService : IRelatorioPdfService
{
    static RelatorioPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<Stream> GerarHoleritePdfAsync(HoleriteDto dados, CancellationToken ct)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Tagged PDF for accessibility (PDF/UA)
                page.Content().Column(col =>
                {
                    // Cabeçalho
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(text =>
                            {
                                text.Span(dados.NomeEmpresa).FontSize(14).Bold();
                            });
                            c.Item().Text(text =>
                            {
                                text.Span($"CNPJ: {dados.CnpjEmpresa}").FontSize(9);
                            });
                        });
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(1);

                    // Dados do funcionário
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(text =>
                            {
                                text.Span($"Funcionário: {dados.NomeFuncionario}").Bold();
                            });
                            c.Item().Text($"Cargo: {dados.Cargo}");
                        });
                        row.ConstantItem(150).Column(c =>
                        {
                            c.Item().Text($"Período: {dados.Periodo}");
                        });
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(1);

                    // Vencimentos
                    col.Item().Text(text =>
                    {
                        text.Span("VENCIMENTOS").FontSize(11).Bold();
                    });
                    col.Item().PaddingVertical(2);
                    foreach (var v in dados.Vencimentos)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"{v.Codigo} - {v.Nome}");
                            row.ConstantItem(100).AlignRight().Text($"{v.Valor:N2}");
                        });
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(0.5f);
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Total Vencimentos").Bold();
                        });
                        row.ConstantItem(100).AlignRight().Text(text =>
                        {
                            text.Span($"{dados.TotalVencimentos:N2}").Bold();
                        });
                    });

                    col.Item().PaddingVertical(8);

                    // Descontos
                    col.Item().Text(text =>
                    {
                        text.Span("DESCONTOS").FontSize(11).Bold();
                    });
                    col.Item().PaddingVertical(2);
                    foreach (var d in dados.Descontos)
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"{d.Codigo} - {d.Nome}");
                            row.ConstantItem(100).AlignRight().Text($"({d.Valor:N2})");
                        });
                    }

                    col.Item().PaddingVertical(2).LineHorizontal(0.5f);
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Total Descontos").Bold();
                        });
                        row.ConstantItem(100).AlignRight().Text(text =>
                        {
                            text.Span($"({dados.TotalDescontos:N2})").Bold();
                        });
                    });

                    col.Item().PaddingVertical(8);

                    // Bases
                    col.Item().Text(text =>
                    {
                        text.Span("BASES DE CÁLCULO").FontSize(11).Bold();
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Base INSS");
                        row.ConstantItem(100).AlignRight().Text($"{dados.BaseInss:N2}");
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Base FGTS");
                        row.ConstantItem(100).AlignRight().Text($"{dados.BaseFgts:N2}");
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Base IRRF");
                        row.ConstantItem(100).AlignRight().Text($"{dados.BaseIrrf:N2}");
                    });

                    col.Item().PaddingVertical(8).LineHorizontal(2);

                    // Líquido
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("VALOR LÍQUIDO").FontSize(14).Bold();
                        });
                        row.ConstantItem(120).AlignRight().Text(text =>
                        {
                            text.Span($"{dados.Liquido:N2}").FontSize(14).Bold();
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1);

                    // Rodapé
                    col.Item().AlignCenter().Text(text =>
                    {
                        text.Span($"Emitido em {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).Italic();
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" de ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }

    public Task<Stream> GerarFolhaAnaliticaPdfAsync(FolhaAnaliticaDto dados, CancellationToken ct)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Content().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span($"FOLHA ANALÍTICA — Período: {dados.Periodo}").FontSize(12).Bold();
                    });
                    col.Item().PaddingVertical(5);

                    foreach (var func in dados.Funcionarios)
                    {
                        col.Item().Text($"{func.Nome} — {func.Departamento}").FontSize(10).Bold();
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Vencimentos: " + string.Join(", ", func.Vencimentos.Select(v => $"{v.Codigo}: {v.Valor:N2}")));
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Descontos: " + string.Join(", ", func.Descontos.Select(d => $"{d.Codigo}: {d.Valor:N2}")));
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Líquido: {func.Liquido:N2}").Bold();
                        });
                        col.Item().PaddingVertical(2).LineHorizontal(0.5f);
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                    text.Span(" de ").FontSize(8);
                    text.TotalPages().FontSize(8);
                });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }

    public Task<Stream> GerarFolhaSinteticaPdfAsync(FolhaSinteticaDto dados, CancellationToken ct)
    {
        return GerarRelatorioPdfAsync(new RelatorioDto
        {
            Titulo = $"FOLHA SINTÉTICA — Período: {dados.Periodo}",
            EmpresaId = dados.EmpresaId,
            Periodo = dados.Periodo,
            Cabecalhos = new List<string> { "Código", "Nome", "Natureza", "Valor" },
            Linhas = dados.TotaisPorRubrica.Select(r => new List<string>
            {
                r.Codigo, r.Nome, r.Natureza, r.Valor.ToString("N2"),
            }).ToList(),
        }, ct);
    }

    public Task<Stream> GerarResumoMensalPdfAsync(ResumoMensalDto dados, CancellationToken ct)
    {
        return GerarRelatorioPdfAsync(new RelatorioDto
        {
            Titulo = $"RESUMO MENSAL — {dados.Periodo}",
            EmpresaId = dados.EmpresaId,
            Periodo = dados.Periodo,
            Cabecalhos = new List<string> { "Indicador", "Valor" },
            Linhas = new List<List<string>>
            {
                new() { "Total Funcionários", dados.TotalFuncionarios.ToString() },
                new() { "Total Vencimentos", dados.TotalVencimentos.ToString("N2") },
                new() { "Total Descontos", dados.TotalDescontos.ToString("N2") },
                new() { "Total Líquido", dados.TotalLiquido.ToString("N2") },
                new() { "Total IRRF", dados.TotalIrrf.ToString("N2") },
                new() { "Total INSS", dados.TotalInss.ToString("N2") },
                new() { "Total FGTS", dados.TotalFgts.ToString("N2") },
            },
        }, ct);
    }

    public Task<Stream> GerarResumoAnualPdfAsync(ResumoAnualDto dados, CancellationToken ct)
    {
        return GerarRelatorioPdfAsync(new RelatorioDto
        {
            Titulo = $"RESUMO ANUAL — {dados.Ano}",
            EmpresaId = dados.EmpresaId,
            Periodo = dados.Ano.ToString(),
            Cabecalhos = new List<string> { "Mês", "Vencimentos", "Descontos", "Líquido" },
            Linhas = dados.Meses.Select(m => new List<string>
            {
                m.Periodo, m.TotalVencimentos.ToString("N2"), m.TotalDescontos.ToString("N2"), m.TotalLiquido.ToString("N2"),
            }).ToList(),
        }, ct);
    }

    public Task<Stream> GerarRelatorioPdfAsync(RelatorioDto dados, CancellationToken ct)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Content().Column(col =>
                {
                    col.Item().Text(text =>
                    {
                        text.Span(dados.Titulo).FontSize(12).Bold();
                    });
                    if (!string.IsNullOrEmpty(dados.NomeEmpresa))
                    {
                        col.Item().Text(text =>
                        {
                            text.Span($"Empresa: {dados.NomeEmpresa}").FontSize(9);
                        });
                    }

                    col.Item().PaddingVertical(5);

                    if (dados.Cabecalhos.Count > 0 && dados.Linhas.Count > 0)
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var unused in dados.Cabecalhos)
                                {
                                    columns.RelativeColumn();
                                }
                            });

                            // Header
                            table.Header(header =>
                            {
                                foreach (var cab in dados.Cabecalhos)
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text(cab).Bold();
                            });

                            // Data rows
                            foreach (var linha in dados.Linhas)
                            {
                                foreach (var cell in linha)
                                    table.Cell().BorderBottom(0.5f).Padding(3).Text(cell);
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ").FontSize(7);
                    text.CurrentPageNumber().FontSize(7);
                    text.Span(" de ").FontSize(7);
                    text.TotalPages().FontSize(7);
                    text.Span($" — Emitido em {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(7);
                });
            });
        });

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return Task.FromResult<Stream>(stream);
    }
}
