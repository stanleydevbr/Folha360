using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class CsllRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveAplicarAliquotaPadrao_ParaLucroPresumido()
    {
        var service = new CsllRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.CSLL, "{\"AliquotaLucroReal\":0.15,\"AliquotaPadrao\":0.09}", "9010");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(4500m, result.ValorDevido); // 50000 * 0.09
        Assert.Equal(0.09m, result.Aliquota);
    }

    [Fact]
    public void Calcular_DeveAplicarAliquotaLucroReal()
    {
        var service = new CsllRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Real",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.CSLL, "{\"AliquotaLucroReal\":0.15,\"AliquotaPadrao\":0.09}", "9010");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(7500m, result.ValorDevido); // 50000 * 0.15
        Assert.Equal(0.15m, result.Aliquota);
    }
}
