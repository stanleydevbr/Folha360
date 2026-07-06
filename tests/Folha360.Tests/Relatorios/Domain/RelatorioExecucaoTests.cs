using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;

namespace Folha360.Tests.Relatorios.Domain;

[Trait("Category", "Unit")]
public class RelatorioExecucaoTests
{
    [Fact]
    public void Iniciar_DeveCriarExecucaoEmAndamento()
    {
        // Arrange
        var agendamentoId = Guid.NewGuid();

        // Act
        var execucao = RelatorioExecucao.Iniciar(agendamentoId);

        // Assert
        Assert.NotEqual(Guid.Empty, execucao.Id);
        Assert.Equal(agendamentoId, execucao.AgendamentoId);
        Assert.Equal(StatusExecucao.EmAndamento, execucao.Status);
        Assert.Null(execucao.ConcluidoEm);
        Assert.Null(execucao.LinkArquivo);
        Assert.Null(execucao.LogErros);
    }

    [Fact]
    public void Concluir_DeveAtualizarStatusEConcluidoEm()
    {
        // Arrange
        var execucao = RelatorioExecucao.Iniciar(Guid.NewGuid());

        // Act
        execucao.Concluir("https://minio/arquivo.pdf");

        // Assert
        Assert.Equal(StatusExecucao.Concluido, execucao.Status);
        Assert.NotNull(execucao.ConcluidoEm);
        Assert.Equal("https://minio/arquivo.pdf", execucao.LinkArquivo);
        Assert.Null(execucao.LogErros);
    }

    [Fact]
    public void Concluir_SemLinkArquivo_DeveManterLinkNulo()
    {
        // Arrange
        var execucao = RelatorioExecucao.Iniciar(Guid.NewGuid());

        // Act
        execucao.Concluir();

        // Assert
        Assert.Equal(StatusExecucao.Concluido, execucao.Status);
        Assert.NotNull(execucao.ConcluidoEm);
        Assert.Null(execucao.LinkArquivo);
    }

    [Fact]
    public void Falhar_DeveAtualizarStatusERegistrarErros()
    {
        // Arrange
        var execucao = RelatorioExecucao.Iniciar(Guid.NewGuid());
        var logErros = "SMTP connection timeout after 30s";

        // Act
        execucao.Falhar(logErros);

        // Assert
        Assert.Equal(StatusExecucao.Falha, execucao.Status);
        Assert.NotNull(execucao.ConcluidoEm);
        Assert.Equal(logErros, execucao.LogErros);
        Assert.Null(execucao.LinkArquivo);
    }

    [Fact]
    public void Iniciar_DiferentesChamadas_DevemGerarIdsDiferentes()
    {
        // Act
        var e1 = RelatorioExecucao.Iniciar(Guid.NewGuid());
        var e2 = RelatorioExecucao.Iniciar(Guid.NewGuid());

        // Assert
        Assert.NotEqual(e1.Id, e2.Id);
    }

    [Fact]
    public void FluxoCompleto_DeveTransitarEstadosCorretamente()
    {
        // Arrange
        var execucao = RelatorioExecucao.Iniciar(Guid.NewGuid());
        Assert.Equal(StatusExecucao.EmAndamento, execucao.Status);

        // Act: concluir
        execucao.Concluir("link");
        Assert.Equal(StatusExecucao.Concluido, execucao.Status);

        // Act: falhar após concluir (simula retry)
        execucao.Falhar("erro");
        Assert.Equal(StatusExecucao.Falha, execucao.Status);
    }
}
