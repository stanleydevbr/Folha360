using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Services;

namespace Folha360.Tests.Cadastros.Domain.Services;

public class AplicadorTabelaProgressivaTests
{
    private readonly AplicadorTabelaProgressiva _aplicador = new();

    [Fact]
    public void Aplicar_IRRF2026_Salario3000_DeveCalcularCorretamente()
    {
        // Arrange — Tabela IRRF 2026
        var faixas = new List<RubricaTabelaProgressiva>
        {
            new(Guid.NewGuid(), 2026, 0m, 2259.20m, 0m, 0m, 1),
            new(Guid.NewGuid(), 2026, 2259.21m, 2826.65m, 7.5m, 169.44m, 2),
            new(Guid.NewGuid(), 2026, 2826.66m, 3751.05m, 15m, 381.44m, 3),
            new(Guid.NewGuid(), 2026, 3751.06m, 4664.68m, 22.5m, 662.77m, 4),
            new(Guid.NewGuid(), 2026, 4664.69m, null, 27.5m, 896.00m, 5),
        };

        // Act — Salário R$ 3.000 (faixa 3: 15%)
        var resultado = _aplicador.Aplicar(3000m, faixas);

        // IRRF esperado: cálculo progressivo
        // Faixa 1: 2259.20 * 0% = 0
        // Faixa 2: (2826.65 - 2259.20) * 7.5% = 567.45 * 0.075 = 42.56
        // Faixa 3: (3000 - 2826.65) * 15% = 173.35 * 0.15 = 26.00
        // Total: 0 + 42.56 + 26.00 = 68.56
        var esperado = 68.56m;
        Assert.Equal(esperado, Math.Round(resultado, 2));
    }

    [Fact]
    public void Aplicar_Isento_DeveRetornarZero()
    {
        var faixas = new List<RubricaTabelaProgressiva>
        {
            new(Guid.NewGuid(), 2026, 0m, 2259.20m, 0m, 0m, 1),
            new(Guid.NewGuid(), 2026, 2259.21m, 2826.65m, 7.5m, 169.44m, 2),
        };

        var resultado = _aplicador.Aplicar(2000m, faixas);
        Assert.Equal(0m, resultado);
    }

    [Fact]
    public void Aplicar_TetoINSS_DeveCalcularCorretamente()
    {
        var faixas = new List<RubricaTabelaProgressiva>
        {
            new(Guid.NewGuid(), 2026, 0m, 1412.00m, 7.5m, 0m, 1),
            new(Guid.NewGuid(), 2026, 1412.01m, 2666.68m, 9m, 0m, 2),
            new(Guid.NewGuid(), 2026, 2666.69m, 4000.03m, 12m, 0m, 3),
            new(Guid.NewGuid(), 2026, 4000.04m, 7786.02m, 14m, 0m, 4),
        };

        var resultado = _aplicador.Aplicar(5000m, faixas);

        // Faixa 1: 1412 * 7.5% = 105.90
        // Faixa 2: (2666.68 - 1412) * 9% = 1254.68 * 0.09 = 112.92
        // Faixa 3: (4000.03 - 2666.68) * 12% = 1333.35 * 0.12 = 160.00
        // Faixa 4: (5000 - 4000.03) * 14% = 999.97 * 0.14 = 140.00
        // Total: 105.90 + 112.92 + 160.00 + 140.00 = 518.82
        var esperado = 518.82m;
        Assert.Equal(esperado, Math.Round(resultado, 2));
    }

    [Fact]
    public void Aplicar_SemFaixas_DeveRetornarZero()
    {
        var resultado = _aplicador.Aplicar(3000m, new List<RubricaTabelaProgressiva>());
        Assert.Equal(0m, resultado);
    }
}
