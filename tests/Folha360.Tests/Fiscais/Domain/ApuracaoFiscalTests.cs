using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Entities;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class ApuracaoFiscalTests
{
    [Fact]
    public void Construtor_DeveCriarApuracaoComStatusPendente()
    {
        var apuracao = new ApuracaoFiscal(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            Tributo.IRRF);

        Assert.NotEqual(Guid.Empty, apuracao.Id);
        Assert.Equal(StatusApuracao.Pendente, apuracao.Status);
    }

    [Fact]
    public void Iniciar_DeveAtualizarStatusParaEmProcessamento()
    {
        var apuracao = new ApuracaoFiscal(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            Tributo.INSS);

        apuracao.Iniciar();

        Assert.Equal(StatusApuracao.EmProcessamento, apuracao.Status);
    }

    [Fact]
    public void Concluir_DeveAtualizarValoresEStatus()
    {
        var apuracao = new ApuracaoFiscal(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            Tributo.FGTS);
        var regraId = Guid.NewGuid();

        apuracao.Iniciar();
        apuracao.Concluir(10000m, 0.08m, 800m, new DateOnly(2026, 7, 7), regraId);

        Assert.Equal(StatusApuracao.Concluido, apuracao.Status);
        Assert.Equal(10000m, apuracao.BaseCalculo);
        Assert.Equal(0.08m, apuracao.Aliquota);
        Assert.Equal(800m, apuracao.ValorDevido);
        Assert.Equal(regraId, apuracao.RegraFiscalId);
    }

    [Fact]
    public void Falhar_DeveAtualizarStatusParaFalho()
    {
        var apuracao = new ApuracaoFiscal(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            Tributo.PIS);

        apuracao.Falhar();

        Assert.Equal(StatusApuracao.Falho, apuracao.Status);
    }

    [Fact]
    public void Reverter_DeveAtualizarStatusParaRevertido()
    {
        var apuracao = new ApuracaoFiscal(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            Tributo.CSLL);

        apuracao.Reverter();

        Assert.Equal(StatusApuracao.Revertido, apuracao.Status);
    }
}
