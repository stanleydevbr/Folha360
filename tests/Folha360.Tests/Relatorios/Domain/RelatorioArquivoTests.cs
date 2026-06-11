using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;

namespace Folha360.Tests.Relatorios.Domain;

[Trait("Category", "Unit")]
public class RelatorioArquivoTests
{
    [Fact]
    public void Registrar_DeveCriarArquivoComPropriedadesCorretas()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var tipoRelatorio = TipoRelatorio.Holerite;
        var periodo = "2026-05";
        var formato = FormatoExportacao.Pdf;
        var bucket = "folha360-holerites";
        var chave = "empresa/2026-05/funcionario-123.pdf";
        var tamanhoBytes = 150_000L;

        // Act
        var arquivo = RelatorioArquivo.Registrar(empresaId, tipoRelatorio, periodo, formato, bucket, chave, tamanhoBytes);

        // Assert
        Assert.NotEqual(Guid.Empty, arquivo.Id);
        Assert.Equal(empresaId, arquivo.EmpresaId);
        Assert.Equal(tipoRelatorio, arquivo.TipoRelatorio);
        Assert.Equal(periodo, arquivo.Periodo);
        Assert.Equal(formato, arquivo.Formato);
        Assert.Equal(bucket, arquivo.Bucket);
        Assert.Equal(chave, arquivo.Chave);
        Assert.Equal(tamanhoBytes, arquivo.TamanhoBytes);
        Assert.Null(arquivo.DeletedAt);
    }

    [Fact]
    public void Invalidar_DevePreencherDeletedAt()
    {
        // Arrange
        var arquivo = RelatorioArquivo.Registrar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, "2026-05",
            FormatoExportacao.Csv, "folha360-relatorios", "chave/teste.csv", 50_000L);

        // Act
        arquivo.Invalidar();

        // Assert
        Assert.NotNull(arquivo.DeletedAt);
    }

    [Fact]
    public void Invalidar_ChamadaDupla_DeveSobrescreverDeletedAt()
    {
        // Arrange
        var arquivo = RelatorioArquivo.Registrar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, "2026-05",
            FormatoExportacao.Csv, "folha360-relatorios", "chave/teste.csv", 50_000L);

        // Act
        arquivo.Invalidar();
        var primeiraInvalidacao = arquivo.DeletedAt;

        // Aguarda um pequeno intervalo para garantir diferença de timestamp
        Thread.Sleep(10);
        arquivo.Invalidar();
        var segundaInvalidacao = arquivo.DeletedAt;

        // Assert
        Assert.NotNull(primeiraInvalidacao);
        Assert.NotNull(segundaInvalidacao);
        Assert.True(segundaInvalidacao > primeiraInvalidacao);
    }

    [Fact]
    public void Registrar_DiferentesChamadas_DevemGerarIdsDiferentes()
    {
        // Act
        var a1 = RelatorioArquivo.Registrar(Guid.NewGuid(), TipoRelatorio.Holerite, "2026-05", FormatoExportacao.Pdf, "b", "k1", 100);
        var a2 = RelatorioArquivo.Registrar(Guid.NewGuid(), TipoRelatorio.Holerite, "2026-05", FormatoExportacao.Pdf, "b", "k2", 200);

        // Assert
        Assert.NotEqual(a1.Id, a2.Id);
    }
}
