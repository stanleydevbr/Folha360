using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Services;

namespace Folha360.Tests.Cadastros.Domain.Services;

public class MotorCalculoTests
{
    private readonly MotorCalculo _motor;

    public MotorCalculoTests()
    {
        var avaliador = new FakeExpressionEvaluator();
        var resolvedor = new ResolvedorComposicao();
        var aplicador = new AplicadorTabelaProgressiva();
        var calculadorMedia = new CalculadorMedia();
        var condicional = new AvaliadorCondicional();
        _motor = new MotorCalculo(avaliador, resolvedor, aplicador, calculadorMedia, condicional);
    }

    [Fact]
    public void Calcular_SalarioBaseFixo_DeveRetornarValorCorreto()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10, ordemExibicao: 1)
        };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(3000m, resultado.TotalVencimentos);
        Assert.Equal(0m, resultado.TotalDescontos);
        Assert.Equal(3000m, resultado.Liquido);
        Assert.Empty(resultado.Erros);
    }

    [Fact]
    public void Calcular_SalarioComHoraExtra_DeveSomarCorretamente()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10),
            new(Guid.NewGuid(), "HE-50", "Horas Extras 50%", "Vencimento",
                tipoCalculo: "HORA", ordemCalculo: 20)
        };
        var contexto = new Dictionary<string, object>
        {
            ["salario_base"] = 3000m,
            ["quantidade_horas"] = 10m,
            ["valor_hora"] = 20m
        };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(3200m, resultado.TotalVencimentos); // 3000 + (10 * 20)
        Assert.Equal(3200m, resultado.Liquido);
    }

    [Fact]
    public void Calcular_DescontoPercentual_DeveSubtrairCorretamente()
    {
        // Arrange
        var salBase = new Rubrica(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
            tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10);
        var vt = new Rubrica(Guid.NewGuid(), "VT", "Vale Transporte 6%", "Desconto",
            tipoCalculo: "PERCENTUAL", percentual: 6m, rubricaBaseId: salBase.Id,
            prioridadeDesconto: 10, ordemCalculo: 210);
        var rubricas = new List<Rubrica> { salBase, vt };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(3000m, resultado.TotalVencimentos);
        Assert.Equal(180m, resultado.TotalDescontos);
        Assert.Equal(2820m, resultado.Liquido);
    }

    [Fact]
    public void Calcular_TabelaProgressivaIRRF_DeveAplicarCorretamente()
    {
        // Arrange
        var salBase = new Rubrica(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
            tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10);

        var rubricaIrrf = new Rubrica(Guid.NewGuid(), "IRRF", "IRRF", "Desconto",
            tipoCalculo: "TABELA_PROGRESSIVA", rubricaBaseId: salBase.Id,
            prioridadeDesconto: 2, ordemCalculo: 201);
        rubricaIrrf.TabelasProgressivas.Add(new RubricaTabelaProgressiva(rubricaIrrf.Id, 2026, 0m, 2259.20m, 0m, 0m, 1));
        rubricaIrrf.TabelasProgressivas.Add(new RubricaTabelaProgressiva(rubricaIrrf.Id, 2026, 2259.21m, 2826.65m, 7.5m, 169.44m, 2));
        rubricaIrrf.TabelasProgressivas.Add(new RubricaTabelaProgressiva(rubricaIrrf.Id, 2026, 2826.66m, 3751.05m, 15m, 381.44m, 3));

        var rubricas = new List<Rubrica> { salBase, rubricaIrrf };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // IRRF sobre R$3000: cálculo progressivo
        // Faixa 1: 2259.20 * 0% = 0
        // Faixa 2: (2826.65 - 2259.20) * 7.5% = 42.56
        // Faixa 3: (3000 - 2826.65) * 15% = 26.00
        // Total: 68.56
        var irrfEsperado = 68.56m;
        Assert.Equal(irrfEsperado, resultado.TotalDescontos, 2);
        Assert.Equal(3000m - irrfEsperado, resultado.Liquido, 2);
    }

    [Fact]
    public void Calcular_TetoMaximo_DeveLimitarValor()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 5000m, tetoMaximo: 4000m, ordemCalculo: 10)
        };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 5000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(4000m, resultado.TotalVencimentos); // Limitado pelo teto
    }

    [Fact]
    public void Calcular_PisoMinimo_DeveGarantirValor()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 500m, pisoMinimo: 1412m, ordemCalculo: 10)
        };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 500m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(1412m, resultado.TotalVencimentos); // Garantido pelo piso
    }

    [Fact]
    public void Calcular_FormulaSimples_DeveAvaliarCorretamente()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10),
            new(Guid.NewGuid(), "BONUS", "Bonus 10%", "Vencimento",
                tipoCalculo: "FORMULA", formulaCalculo: "SALARIO_BASE * 0.10", ordemCalculo: 30)
        };
        var contexto = new Dictionary<string, object> { ["SALARIO_BASE"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(3300m, resultado.TotalVencimentos); // 3000 + 300
    }

    [Fact]
    public void Calcular_RubricaInativa_DeveIgnorar()
    {
        // Arrange
        var rubricas = new List<Rubrica>
        {
            new(Guid.NewGuid(), "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10),
            new(Guid.NewGuid(), "BONUS-INATIVO", "Bonus Inativo", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 1000m, ativo: false, ordemCalculo: 30)
        };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert
        Assert.Equal(3000m, resultado.TotalVencimentos); // Bonus inativo ignorado
    }

    [Fact]
    public void Calcular_OrdemProcessamento_DeveRespeitarFases()
    {
        // Arrange
        var rubricaBaseId = Guid.NewGuid();
        var rubricas = new List<Rubrica>
        {
            new(rubricaBaseId, "SAL-BASE", "Salario Base", "Vencimento",
                tipoCalculo: "VALOR_FIXO", valorFixo: 3000m, ordemCalculo: 10),

            // BASE-INSS na fase 2 (ordem 100-199)
            new(Guid.NewGuid(), "BASE-INSS", "Base INSS", "Base",
                tipoCalculo: "COMPOSICAO", ordemCalculo: 100),

            // INSS na fase 3
            new(Guid.NewGuid(), "INSS", "INSS", "Desconto",
                tipoCalculo: "TABELA_PROGRESSIVA", rubricaBaseId: rubricaBaseId,
                prioridadeDesconto: 1, ordemCalculo: 200),

            // TOTAL-VENC na fase 4
            new(Guid.NewGuid(), "TOTAL-VENC", "Total Vencimentos", "Informativo",
                tipoCalculo: "COMPOSICAO", ordemCalculo: 300)
        };
        var contexto = new Dictionary<string, object> { ["salario_base"] = 3000m };

        // Act
        var resultado = _motor.Calcular(rubricas, contexto);

        // Assert — sem erro de ordem
        Assert.Equal(3000m, resultado.TotalVencimentos);
    }
}

/// <summary>
/// Fake expression evaluator for unit tests — simulates NCalc without dependency.
/// </summary>
internal class FakeExpressionEvaluator : IExpressionEvaluator
{
    public decimal Avaliar(string expressao, IReadOnlyDictionary<string, object> parametros)
    {
        // Simple evaluator for test formulas
        if (expressao.Contains("SALARIO_BASE * 0.10"))
            return 300m;

        if (expressao.Contains("SALARIO_BASE"))
        {
            var salario = parametros.TryGetValue("SALARIO_BASE", out var s) ? Convert.ToDecimal(s) : 0m;
            return salario * 0.10m;
        }

        return 0m;
    }
}
