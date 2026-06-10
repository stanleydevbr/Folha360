using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class FgtsRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveAplicar8Porcento()
    {
        var service = new FgtsRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.FGTS, "{}", "115");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(4000m, result.ValorDevido); // 50000 * 0.08
        Assert.Equal(0.08m, result.Aliquota);
        Assert.Equal(new DateOnly(2026, 7, 7), result.DataVencimento);
    }

    [Fact]
    public void Calcular_DeveRetornarZero_QuandoBaseZero()
    {
        var service = new FgtsRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            0,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.FGTS, "{}", "115");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(0m, result.ValorDevido);
    }
}
