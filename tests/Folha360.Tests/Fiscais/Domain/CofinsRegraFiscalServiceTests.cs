using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class CofinsRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveAplicarAliquotaPadrao_ParaLucroPresumido()
    {
        var service = new CofinsRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.COFINS, "{\"AliquotaLucroReal\":0.076,\"AliquotaPadrao\":0.03}", "5678");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(1500m, result.ValorDevido); // 50000 * 0.03
        Assert.Equal(0.03m, result.Aliquota);
    }

    [Fact]
    public void Calcular_DeveAplicarAliquotaLucroReal()
    {
        var service = new CofinsRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Real",
            null,
            new List<Guid>(),
            50000m,
            0);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.COFINS, "{\"AliquotaLucroReal\":0.076,\"AliquotaPadrao\":0.03}", "5678");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(3800m, result.ValorDevido); // 50000 * 0.076
        Assert.Equal(0.076m, result.Aliquota);
    }
}
