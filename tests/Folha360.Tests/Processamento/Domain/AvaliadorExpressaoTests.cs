using Folha360.Processamento.Domain.Services;
using Moq;
using Xunit;

namespace Folha360.Tests.Processamento.Domain;

[Trait("Category", "Unit")]
public class AvaliadorExpressaoTests
{
    private readonly Mock<IExpressionEvaluator> _evaluatorMock;
    private readonly AvaliadorExpressao _avaliador;

    public AvaliadorExpressaoTests()
    {
        _evaluatorMock = new Mock<IExpressionEvaluator>();
        _avaliador = new AvaliadorExpressao(_evaluatorMock.Object);
    }

    [Fact]
    public void Avaliar_ExpressaoVazia_RetornaZero()
    {
        var result = _avaliador.Avaliar(string.Empty, new Dictionary<string, decimal>());
        Assert.Equal(0m, result);
    }

    [Fact]
    public void Avaliar_ExpressaoNula_RetornaZero()
    {
        var result = _avaliador.Avaliar(null!, new Dictionary<string, decimal>());
        Assert.Equal(0m, result);
    }

    [Fact]
    public void Avaliar_DelegaParaExpressionEvaluator()
    {
        var variaveis = new Dictionary<string, decimal> { ["SALARIO_BASE"] = 3000m };
        _evaluatorMock
            .Setup(e => e.Avaliar("SALARIO_BASE * 0.10", It.IsAny<IReadOnlyDictionary<string, object>>()))
            .Returns(300m);

        var result = _avaliador.Avaliar("SALARIO_BASE * 0.10", variaveis);

        Assert.Equal(300m, result);
    }
}
