using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class SindicalRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveRetornarValorTotalFolha()
    {
        var service = new SindicalRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,
            2000m);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.ContribuicaoSindical, "{}", "9999");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(2000m, result.ValorDevido);
        Assert.Equal(new DateOnly(2026, 7, 10), result.DataVencimento);
    }
}
