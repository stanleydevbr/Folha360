using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class IrpfRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveRetornarValorTotalFolha()
    {
        var service = new IrpfRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            15000m);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.IRRF, "{}", "0561");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(15000m, result.ValorDevido);
        Assert.Equal(15000m, result.BaseCalculo);
        Assert.Equal("0561", result.CodigoReceita);
        Assert.Equal(new DateOnly(2026, 7, 20), result.DataVencimento);
    }
}
