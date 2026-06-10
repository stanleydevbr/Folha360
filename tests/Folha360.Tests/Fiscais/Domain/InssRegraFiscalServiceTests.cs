using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class InssRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveCalcularParteEmpregadoEPatronal()
    {
        var service = new InssRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            50000m,  // baseCalculoTotal
            5000m);  // valorTotalFolha (parte empregado)
        var parametros = new RegraFiscalParametros(
            Folha360.Fiscais.Domain.Tributo.INSS,
            "{\"AliquotaPatronal\":0.20,\"Rat\":0.02,\"Fap\":0.01,\"Terceiros\":0.058}",
            "2100");

        var result = service.Calcular(contexto, parametros);

        // aliquota patronal = 0.20 + 0.02 + 0.01 + 0.058 = 0.288
        // parte patronal = 50000 * 0.288 = 14400
        // total = 5000 + 14400 = 19400
        Assert.Equal(19400m, result.ValorDevido);
        Assert.Equal(0.288m, result.Aliquota);
        Assert.Equal(new DateOnly(2026, 7, 20), result.DataVencimento);
    }

    [Fact]
    public void Calcular_DeveUsarDefaults_QuandoJsonVazio()
    {
        var service = new InssRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            null,
            new List<Guid>(),
            100000m,
            10000m);
        var parametros = new RegraFiscalParametros(Folha360.Fiscais.Domain.Tributo.INSS, "{}", "2100");

        var result = service.Calcular(contexto, parametros);

        // defaults: AliquotaPatronal=0.20, Rat=0.02, Fap=0.01, Terceiros=0.058 => 0.288
        Assert.Equal(10000m + (100000m * 0.288m), result.ValorDevido);
    }
}
