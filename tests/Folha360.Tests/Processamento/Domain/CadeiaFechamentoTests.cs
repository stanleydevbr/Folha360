using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Entities;
using Xunit;

namespace Folha360.Tests.Processamento.Domain;

[Trait("Category", "Unit")]
public class CadeiaFechamentoTests
{
    [Fact]
    public void Construtor_DeveCriarComEtapaFolhaProcessada()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, cadeia.Id);
        Assert.Equal(EtapaFechamento.FolhaProcessada, cadeia.Etapa);
        Assert.Equal(StatusEtapa.Pendente, cadeia.Status);
        Assert.Equal(1, cadeia.Versao);
    }

    [Fact]
    public void AvancarEtapa_DeveAtualizarEtapaEStatus()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        cadeia.AvancarEtapa(EtapaFechamento.ObrigacoesApuradas);

        Assert.Equal(EtapaFechamento.ObrigacoesApuradas, cadeia.Etapa);
        Assert.Equal(StatusEtapa.Concluido, cadeia.Status);
    }

    [Fact]
    public void AvancarEtapa_FechamentoConcluido_DeveDefinirDataFim()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        cadeia.AvancarEtapa(EtapaFechamento.FechamentoConcluido);

        Assert.NotNull(cadeia.DataFim);
    }

    [Fact]
    public void FalharEtapa_DeveAtualizarStatusComErro()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        cadeia.FalharEtapa("Timeout na apuração fiscal");

        Assert.Equal(StatusEtapa.Falhou, cadeia.Status);
        Assert.Equal("Timeout na apuração fiscal", cadeia.Erro);
    }

    [Fact]
    public void Compensar_DeveAtualizarStatusParaCompensando()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        cadeia.Compensar();

        Assert.Equal(StatusEtapa.Compensando, cadeia.Status);
    }

    [Fact]
    public void Reabrir_DeveIncrementarVersao()
    {
        var cadeia = new CadeiaFechamento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid());

        cadeia.Reabrir("[{\"versao\":1,\"status\":\"Concluido\"}]");

        Assert.Equal(StatusEtapa.Reaberta, cadeia.Status);
        Assert.Equal(2, cadeia.Versao);
        Assert.Equal("[{\"versao\":1,\"status\":\"Concluido\"}]", cadeia.HistoricoVersoes);
    }
}
