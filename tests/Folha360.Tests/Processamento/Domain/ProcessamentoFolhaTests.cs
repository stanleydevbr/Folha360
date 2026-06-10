using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Entities;
using Xunit;

namespace Folha360.Tests.Processamento.Domain;

[Trait("Category", "Unit")]
public class ProcessamentoFolhaTests
{
    [Fact]
    public void Construtor_DeveCriarProcessamentoComStatusPendente()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        Assert.NotEqual(Guid.Empty, processamento.Id);
        Assert.Equal(StatusProcessamento.Pendente, processamento.Status);
        Assert.Equal(1, processamento.Versao);
    }

    [Fact]
    public void Iniciar_DeveAtualizarStatusParaEmProcessamento()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        processamento.Iniciar(100);

        Assert.Equal(StatusProcessamento.EmProcessamento, processamento.Status);
        Assert.Equal(100, processamento.TotalFuncionarios);
        Assert.NotNull(processamento.DataInicio);
    }

    [Fact]
    public void Concluir_DeveAtualizarTotaisEStatus()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        processamento.Iniciar(10);
        processamento.Concluir(5000m, 1000m, 4000m, 400m);

        Assert.Equal(StatusProcessamento.Concluido, processamento.Status);
        Assert.Equal(5000m, processamento.TotalVencimentos);
        Assert.Equal(1000m, processamento.TotalDescontos);
        Assert.Equal(4000m, processamento.TotalLiquido);
        Assert.Equal(400m, processamento.TotalFgts);
        Assert.NotNull(processamento.DataFim);
    }

    [Fact]
    public void Falhar_DeveAtualizarStatusComErro()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        processamento.Falhar("Erro de conexão com banco");

        Assert.Equal(StatusProcessamento.Falho, processamento.Status);
        Assert.Equal("Erro de conexão com banco", processamento.Erro);
    }

    [Fact]
    public void Cancelar_DeveAtualizarStatusParaCancelado()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        processamento.Iniciar(10);
        processamento.Cancelar();

        Assert.Equal(StatusProcessamento.Cancelado, processamento.Status);
    }

    [Fact]
    public void Reabrir_DeveAtualizarStatusERegistrarReabertura()
    {
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal);

        processamento.Reabrir("operador@teste.com", "Rubrica com fórmula incorreta corrigida");

        Assert.Equal(StatusProcessamento.Reaberta, processamento.Status);
        Assert.Equal("operador@teste.com", processamento.ReabertoPor);
        Assert.Equal("Rubrica com fórmula incorreta corrigida", processamento.MotivoReabertura);
        Assert.NotNull(processamento.ReabertoEm);
    }

    [Fact]
    public void ConstrutorComVersao_DeveCriarComProcessamentoOriginalId()
    {
        var originalId = Guid.NewGuid();
        var processamento = new ProcessamentoFolha(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            TipoCalculo.Mensal,
            2,
            originalId);

        Assert.Equal(2, processamento.Versao);
        Assert.Equal(originalId, processamento.ProcessamentoOriginalId);
    }
}
