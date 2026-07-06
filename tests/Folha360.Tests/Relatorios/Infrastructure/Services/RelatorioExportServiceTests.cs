using System.Text;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Infrastructure.Services;

namespace Folha360.Tests.Relatorios.Infrastructure.Services;

[Trait("Category", "Unit")]
public class RelatorioExportServiceTests
{
    private readonly RelatorioExportService _service = new();

    [Fact]
    public async Task ExportarCsvAsync_QuandoListaVazia_DeveRetornarStreamVazio()
    {
        // Arrange
        var dados = new List<DirfDto>();

        // Act
        var stream = await _service.ExportarCsvAsync(dados, CancellationToken.None);

        // Assert
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public async Task ExportarCsvAsync_DeveGerarCsvComHeaderEDados()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "12345678901", Nome = "João", RendimentosTributaveis = 50000m, RendimentosIsentos = 10000m, IrrfRetido = 7500m, DecimoTerceiro = 5000m, Ferias = 4000m },
        };

        // Act
        var stream = await _service.ExportarCsvAsync(dados, CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("cpf", csv.ToLowerInvariant());
        Assert.Contains("nome", csv.ToLowerInvariant());
        Assert.Contains("12345678901", csv);
        Assert.Contains("João", csv);
    }

    [Fact]
    public async Task ExportarCsvAsync_DeveUsarDelimitadorPontoEVirgula()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "11111111111", Nome = "Teste", RendimentosTributaveis = 1000m },
        };

        // Act
        var stream = await _service.ExportarCsvAsync(dados, CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains(";", csv);
    }

    [Fact]
    public async Task ExportarCsvAsync_DeveConterBomUtf8()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "11111111111", Nome = "Teste" },
        };

        // Act
        var stream = await _service.ExportarCsvAsync(dados, CancellationToken.None);
        stream.Position = 0;
        var bytes = new byte[3];
        var bytesRead = await stream.ReadAsync(bytes, 0, 3);
        Assert.Equal(3, bytesRead);

        // Assert - BOM UTF-8 = 0xEF, 0xBB, 0xBF
        Assert.Equal(0xEF, bytes[0]);
        Assert.Equal(0xBB, bytes[1]);
        Assert.Equal(0xBF, bytes[2]);
    }

    [Fact]
    public async Task ExportarCsvDirfAsync_DeveSeguirLeiauteOficial()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "12345678901", Nome = "João Silva", RendimentosTributaveis = 50000m, RendimentosIsentos = 10000m, IrrfRetido = 7500m, DecimoTerceiro = 5000m, Ferias = 4000m },
        };

        // Act
        var stream = await _service.ExportarCsvDirfAsync(dados, CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("CPF;Nome;RendimentosTributaveis;RendimentosIsentos;IRRFRetido;13Salario;Ferias", csv);
        Assert.Contains("12345678901", csv);
        Assert.Contains("João Silva", csv);
    }

    [Fact]
    public async Task ExportarCsvRaisAsync_DeveSeguirLeiauteOficial()
    {
        // Arrange
        var dados = new List<RaisDto>
        {
            new()
            {
                Cpf = "98765432100", Nome = "Maria Souza", PisPasep = "12345678901",
                DataAdmissao = new DateTime(2020, 1, 15),
                RemuneracaoJaneiro = 5000m, RemuneracaoTotal = 60000m,
            },
        };

        // Act
        var stream = await _service.ExportarCsvRaisAsync(dados, CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("CPF;Nome;PISPASEP;Admissao", csv);
        Assert.Contains("98765432100", csv);
        Assert.Contains("Maria Souza", csv);
        Assert.Contains("2020-01-15", csv);
    }

    [Fact]
    public async Task ExportarCsvRaisAsync_DeveIncluirTodasRemuneracoesMensais()
    {
        // Arrange
        var dados = new List<RaisDto>
        {
            new()
            {
                Cpf = "11111111111", Nome = "Teste",
                RemuneracaoJaneiro = 1000m, RemuneracaoFevereiro = 2000m, RemuneracaoMarco = 3000m,
                RemuneracaoAbril = 4000m, RemuneracaoMaio = 5000m, RemuneracaoJunho = 6000m,
                RemuneracaoJulho = 7000m, RemuneracaoAgosto = 8000m, RemuneracaoSetembro = 9000m,
                RemuneracaoOutubro = 10000m, RemuneracaoNovembro = 11000m, RemuneracaoDezembro = 12000m,
                RemuneracaoTotal = 78000m,
            },
        };

        // Act
        var stream = await _service.ExportarCsvRaisAsync(dados, CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("RemunJan;RemunFev;RemunMar;RemunAbr;RemunMai;RemunJun;RemunJul;RemunAgo;RemunSet;RemunOut;RemunNov;RemunDez;RemunTotal;13Salario", csv);
    }

    [Fact]
    public async Task ExportarXmlAsync_DeveGerarXmlValido()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "12345678901", Nome = "João", RendimentosTributaveis = 50000m },
        };

        // Act
        var stream = await _service.ExportarXmlAsync(dados, "dirf", CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var xml = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("<?xml", xml);
        Assert.Contains("<dirf>", xml);
        Assert.Contains("<item>", xml);
        Assert.Contains("<cpf>12345678901</cpf>", xml);
    }

    [Fact]
    public async Task ExportarXmlAsync_DeveUsarSnakeCaseNosElementos()
    {
        // Arrange
        var dados = new List<DirfDto>
        {
            new() { Cpf = "11111111111", Nome = "Teste", RendimentosTributaveis = 1000m, IrrfRetido = 100m },
        };

        // Act
        var stream = await _service.ExportarXmlAsync(dados, "relatorio", CancellationToken.None);
        stream.Position = 0;
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var xml = await reader.ReadToEndAsync();

        // Assert
        Assert.Contains("<rendimentos_tributaveis>", xml);
        Assert.Contains("<irrf_retido>", xml);
    }
}
