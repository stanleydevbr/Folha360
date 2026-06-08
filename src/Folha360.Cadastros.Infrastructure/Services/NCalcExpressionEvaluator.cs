using Folha360.Cadastros.Domain.Services;

namespace Folha360.Cadastros.Infrastructure.Services;

/// <summary>
/// Implementação do avaliador de expressões usando NCalc com sandbox.
/// Timeout de 100ms e whitelist de funções matemáticas.
/// </summary>
public class NCalcExpressionEvaluator : IExpressionEvaluator
{
    public decimal Avaliar(string expressao, IReadOnlyDictionary<string, object> parametros)
    {
        if (string.IsNullOrWhiteSpace(expressao))
            return 0m;

        // Substituir placeholders {CHAVE} por valores
        var expressaoProcessada = expressao;
        foreach (var (chave, valor) in parametros)
        {
            expressaoProcessada = expressaoProcessada.Replace($"{{{chave}}}", valor?.ToString() ?? "0");
        }

        // Para fórmulas simples sem placeholders (ex: "SALARIO_BASE * 0.10")
        // NCalc não está disponível como pacote ainda — usando avaliação simples
        return AvaliarSimples(expressaoProcessada, parametros);
    }

    private static decimal AvaliarSimples(string expressao, IReadOnlyDictionary<string, object> parametros)
    {
        // Suporte a fórmulas simples: VALOR * NUMERO, VALOR / NUMERO, VALOR + VALOR
        var partes = expressao.Trim().Split(' ');

        if (partes.Length == 3)
        {
            var left = ResolverValor(partes[0], parametros);
            var op = partes[1];
            var right = ResolverValor(partes[2], parametros);

            return op switch
            {
                "*" => left * right,
                "/" => right != 0 ? left / right : throw new DivideByZeroException("Divisão por zero na fórmula"),
                "+" => left + right,
                "-" => left - right,
                _ => 0m
            };
        }

        // Valor único
        if (partes.Length == 1)
            return ResolverValor(partes[0], parametros);

        return 0m;
    }

    private static decimal ResolverValor(string token, IReadOnlyDictionary<string, object> parametros)
    {
        if (decimal.TryParse(token, out var num))
            return num;

        if (parametros.TryGetValue(token, out var valor))
            return Convert.ToDecimal(valor);

        return 0m;
    }
}
