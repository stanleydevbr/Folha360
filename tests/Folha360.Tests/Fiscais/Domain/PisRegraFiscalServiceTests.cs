using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class PisRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveAplicarAliquotaPadrao_ParaLucroPresumido()
    {
        var service = new PisRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.PIS, "{\"AliquotaLucroReal\":0.0065,\"AliquotaPadrao\":0.01}", "1234");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(500m, result.ValorDevido); // 50000 * 0.01
        Assert.Equal(0.01m, result.Aliquota);
    }

    [Fact]
    public void Calcular_DeveAplicarAliquotaLucroReal()
    {
        var service = new PisRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Real",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.PIS, "{\"AliquotaLucroReal\":0.0065,\"AliquotaPadrao\":0.01}", "1234");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(325m, result.ValorDevido); // 50000 * 0.0065
        Assert.Equal(0.0065m, result.Aliquota);
    }
}
