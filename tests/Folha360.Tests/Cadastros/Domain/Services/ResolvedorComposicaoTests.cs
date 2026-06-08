using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Services;

namespace Folha360.Tests.Cadastros.Domain.Services;

public class ResolvedorComposicaoTests
{
    private readonly ResolvedorComposicao _resolvedor = new();

    [Fact]
    public void Resolver_SomaSimples_DeveRetornarSoma()
    {
        // Arrange
        var rubrica = new Rubrica(Guid.NewGuid(), "TOTAL", "Total", "Informativo", tipoCalculo: "COMPOSICAO");
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "+", ordem: 0));
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "+", ordem: 1));

        var valores = new Dictionary<Guid, decimal>
        {
            [rubrica.Composicoes.First().RubricaComponenteId] = 100m,
            [rubrica.Composicoes.Last().RubricaComponenteId] = 200m
        };

        // Act
        var resultado = _resolvedor.Resolver(rubrica, valores);

        // Assert
        Assert.Equal(300m, resultado);
    }

    [Fact]
    public void Resolver_Subtracao_DeveSubtrair()
    {
        // Arrange
        var rubrica = new Rubrica(Guid.NewGuid(), "LIQUIDO", "Liquido", "Informativo", tipoCalculo: "COMPOSICAO");
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "+"));
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "-"));

        var valores = new Dictionary<Guid, decimal>
        {
            [rubrica.Composicoes.First().RubricaComponenteId] = 3000m,
            [rubrica.Composicoes.Last().RubricaComponenteId] = 500m
        };

        // Act
        var resultado = _resolvedor.Resolver(rubrica, valores);

        // Assert
        Assert.Equal(2500m, resultado);
    }

    [Fact]
    public void Resolver_ComponenteObrigatorioAusente_DeveLancarExcecao()
    {
        // Arrange
        var rubrica = new Rubrica(Guid.NewGuid(), "TOTAL", "Total", "Informativo", tipoCalculo: "COMPOSICAO");
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "+", obrigatorio: true));
        var valores = new Dictionary<Guid, decimal>(); // Componente ausente

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _resolvedor.Resolver(rubrica, valores));
    }

    [Fact]
    public void Resolver_ComponenteOpcionalAusente_DeveIgnorar()
    {
        // Arrange
        var rubrica = new Rubrica(Guid.NewGuid(), "TOTAL", "Total", "Informativo", tipoCalculo: "COMPOSICAO");
        rubrica.Composicoes.Add(new RubricaComposicao(rubrica.Id, Guid.NewGuid(), "+", obrigatorio: false));
        var valores = new Dictionary<Guid, decimal>(); // Componente ausente mas opcional

        // Act
        var resultado = _resolvedor.Resolver(rubrica, valores);

        // Assert
        Assert.Equal(0m, resultado);
    }

    [Fact]
    public void IsCicloSafe_GrafoAciclico_DeveRetornarTrue()
    {
        // Arrange
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();
        var composicoes = new List<RubricaComposicao>
        {
            new(a, b, "+"),
            new(b, c, "+")
        };

        // Act — adicionar A → C (não cria ciclo)
        var safe = _resolvedor.IsCicloSafe(a, c, composicoes);

        // Assert
        Assert.True(safe);
    }

    [Fact]
    public void IsCicloSafe_GrafoCiclico_DeveRetornarFalse()
    {
        // Arrange
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var composicoes = new List<RubricaComposicao>
        {
            new(a, b, "+")
        };

        // Act — adicionar B → A (cria ciclo)
        var safe = _resolvedor.IsCicloSafe(b, a, composicoes);

        // Assert
        Assert.False(safe);
    }
}
