using Folha360.Processamento.Domain.Services;
using Xunit;

namespace Folha360.Tests.Processamento.Domain;

[Trait("Category", "Unit")]
public class AvaliadorCondicionalTests
{
    private readonly AvaliadorCondicional _avaliador = new();

    [Fact]
    public void Avaliar_CondicaoVazia_RetornaTrue()
    {
        var result = _avaliador.Avaliar(string.Empty, new Dictionary<string, object>());
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_CondicaoNula_RetornaTrue()
    {
        var result = _avaliador.Avaliar(null!, new Dictionary<string, object>());
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_ComparacaoMaiorQue_RetornaTrue()
    {
        var contexto = new Dictionary<string, object> { ["SALARIO_BASE"] = 5000m };
        var result = _avaliador.Avaliar("5000 > 3000", contexto);
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_ComparacaoMenorQue_RetornaFalse()
    {
        var contexto = new Dictionary<string, object> { ["SALARIO_BASE"] = 2000m };
        var result = _avaliador.Avaliar("2000 < 3000", contexto);
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_IgualdadeString_RetornaTrue()
    {
        var contexto = new Dictionary<string, object> { ["TIPO_CONTRATO"] = "CLT" };
        var result = _avaliador.Avaliar("CLT == CLT", contexto);
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_AndComDuasCondicoesVerdadeiras_RetornaTrue()
    {
        var result = _avaliador.Avaliar("5000 > 3000 AND 10 > 5", new Dictionary<string, object>());
        Assert.True(result);
    }

    [Fact]
    public void Avaliar_AndComUmaCondicaoFalsa_RetornaFalse()
    {
        var result = _avaliador.Avaliar("5000 > 3000 AND 10 < 5", new Dictionary<string, object>());
        Assert.False(result);
    }

    [Fact]
    public void Avaliar_OrComUmaCondicaoVerdadeira_RetornaTrue()
    {
        var result = _avaliador.Avaliar("5000 < 3000 OR 10 > 5", new Dictionary<string, object>());
        Assert.True(result);
    }
}
