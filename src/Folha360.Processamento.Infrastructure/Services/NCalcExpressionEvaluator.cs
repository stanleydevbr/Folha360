using Folha360.Processamento.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Infrastructure.Services;

public class NCalcExpressionEvaluator : IExpressionEvaluator
{
    private readonly ILogger<NCalcExpressionEvaluator> _logger;

    public NCalcExpressionEvaluator(ILogger<NCalcExpressionEvaluator> logger)
    {
        _logger = logger;
    }

    public decimal Avaliar(string expressao, IReadOnlyDictionary<string, object> parametros)
    {
        if (string.IsNullOrWhiteSpace(expressao))
            return 0m;

        try
        {
            var expressaoProcessada = expressao;
            foreach (var (chave, valor) in parametros)
            {
                expressaoProcessada = expressaoProcessada.Replace(
                    $"{{{chave}}}", valor?.ToString() ?? "0");
            }

            return AvaliarSimples(expressaoProcessada, parametros);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao avaliar expressão: {Expressao}", expressao);
            throw new InvalidOperationException($"Erro ao avaliar expressão: {ex.Message}", ex);
        }
    }

    private static decimal AvaliarSimples(string expressao, IReadOnlyDictionary<string, object> parametros)
    {
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
                _ => 0m,
            };
        }

        if (partes.Length == 1)
        {
            return ResolverValor(partes[0], parametros);
        }

        return 0m;
    }

    private static decimal ResolverValor(string token, IReadOnlyDictionary<string, object> parametros)
    {
        if (decimal.TryParse(token, out var valor))
            return valor;

        if (parametros.TryGetValue(token, out var objValor))
        {
            if (objValor is decimal d)
                return d;
            if (objValor is int i)
                return i;
            if (objValor is double db)
                return (decimal)db;
            if (objValor is long l)
                return l;
            if (objValor is string s && decimal.TryParse(s, out var parsed))
                return parsed;
        }

        return 0m;
    }
}
