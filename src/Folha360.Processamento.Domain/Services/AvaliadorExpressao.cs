using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Processamento.Domain.Services;

public interface IAvaliadorExpressao
{
    decimal Avaliar(string expressao, IReadOnlyDictionary<string, decimal> variaveis);
}

public class AvaliadorExpressao : IAvaliadorExpressao
{
    private readonly IExpressionEvaluator _evaluator;

    public AvaliadorExpressao(IExpressionEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    public decimal Avaliar(string expressao, IReadOnlyDictionary<string, decimal> variaveis)
    {
        if (string.IsNullOrWhiteSpace(expressao))
        {
            return 0;
        }

        var parametros = variaveis.ToDictionary<KeyValuePair<string, decimal>, string, object>(
            kv => kv.Key,
            kv => (object)kv.Value);

        return _evaluator.Avaliar(expressao, parametros);
    }
}
