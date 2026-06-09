using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Entities;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class RegraFiscalTests
{
    [Fact]
    public void Construtor_DeveCriarRegraAtiva()
    {
        var regra = new RegraFiscal(
            Tributo.IRRF,
            2026,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            "{\"faixas\":[{\"limite\":2259.20,\"aliquota\":0}]}",
            "0561");

        Assert.True(regra.Ativo);
        Assert.Equal(Tributo.IRRF, regra.Tributo);
        Assert.Equal(2026, regra.Versao);
    }

    [Fact]
    public void EstaVigente_DeveRetornarTrue_QuandoDataDentroDaVigencia()
    {
        var regra = new RegraFiscal(
            Tributo.INSS,
            2026,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            "{}",
            "2100");

        Assert.True(regra.EstaVigente(new DateOnly(2026, 6, 15)));
    }

    [Fact]
    public void EstaVigente_DeveRetornarFalse_QuandoDataForaDaVigencia()
    {
        var regra = new RegraFiscal(
            Tributo.INSS,
            2026,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            "{}",
            "2100");

        Assert.False(regra.EstaVigente(new DateOnly(2025, 12, 15)));
        Assert.False(regra.EstaVigente(new DateOnly(2027, 1, 15)));
    }

    [Fact]
    public void EstaVigente_DeveRetornarTrue_QuandoVigenciaFimNula()
    {
        var regra = new RegraFiscal(
            Tributo.FGTS,
            2026,
            new DateOnly(2026, 1, 1),
            null,
            "{}",
            "115");

        Assert.True(regra.EstaVigente(new DateOnly(2030, 6, 15)));
    }

    [Fact]
    public void Desativar_DeveTornarRegraInativa()
    {
        var regra = new RegraFiscal(
            Tributo.PIS,
            2026,
            new DateOnly(2026, 1, 1),
            null,
            "{}",
            "1234");

        regra.Desativar();

        Assert.False(regra.Ativo);
    }

    [Fact]
    public void Ativar_DeveTornarRegraAtiva()
    {
        var regra = new RegraFiscal(
            Tributo.PIS,
            2026,
            new DateOnly(2026, 1, 1),
            null,
            "{}",
            "1234");
        regra.Desativar();

        regra.Ativar();

        Assert.True(regra.Ativo);
    }
}
