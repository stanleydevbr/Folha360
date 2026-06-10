using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class IssRegraFiscalServiceTests
{
    [Fact]
    public void Calcular_DeveAplicarAliquotaPadrao_QuandoMunicipioNaoEncontrado()
    {
        var service = new IssRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            "Cidade Desconhecida",
            new List<Guid>(),
            10000m,
            0);
        var parametros = new RegraFiscalParametros(
            Folha360.Fiscais.Domain.Tributo.ISS,
            "{\"AliquotaPadrao\":0.05,\"AliquotasPorMunicipio\":[{\"Municipio\":\"São Paulo\",\"Aliquota\":0.05},{\"Municipio\":\"Belo Horizonte\",\"Aliquota\":0.03}]}",
            "9999");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(500m, result.ValorDevido); // 10000 * 0.05
        Assert.Equal(0.05m, result.Aliquota);
    }

    [Fact]
    public void Calcular_DeveAplicarAliquotaEspecifica_PorMunicipio()
    {
        var service = new IssRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            "Belo Horizonte",
            new List<Guid>(),
            10000m,
            0);
        var parametros = new RegraFiscalParametros(
            Folha360.Fiscais.Domain.Tributo.ISS,
            "{\"AliquotaPadrao\":0.05,\"AliquotasPorMunicipio\":[{\"Municipio\":\"São Paulo\",\"Aliquota\":0.05},{\"Municipio\":\"Belo Horizonte\",\"Aliquota\":0.03}]}",
            "9999");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(300m, result.ValorDevido); // 10000 * 0.03
        Assert.Equal(0.03m, result.Aliquota);
    }

    [Fact]
    public void Calcular_DeveAplicarAliquotaSaoPaulo()
    {
        var service = new IssRegraFiscalService();
        var contexto = new ApuracaoContext(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            "Lucro Presumido",
            "São Paulo",
            new List<Guid>(),
            10000m,
            0);
        var parametros = new RegraFiscalParametros(
            Folha360.Fiscais.Domain.Tributo.ISS,
            "{\"AliquotaPadrao\":0.05,\"AliquotasPorMunicipio\":[{\"Municipio\":\"São Paulo\",\"Aliquota\":0.05},{\"Municipio\":\"Belo Horizonte\",\"Aliquota\":0.03}]}",
            "9999");

        var result = service.Calcular(contexto, parametros);

        Assert.Equal(500m, result.ValorDevido); // 10000 * 0.05
    }
}
