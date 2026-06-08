namespace Folha360.Processamento.Domain.Services;

public interface IAvaliadorCondicional
{
    bool Avaliar(string condicao, IReadOnlyDictionary<string, object> contexto);
}

public class AvaliadorCondicional : IAvaliadorCondicional
{
    public bool Avaliar(string condicao, IReadOnlyDictionary<string, object> contexto)
    {
        if (string.IsNullOrWhiteSpace(condicao))
            return true;

        var expressao = condicao;
        var chavesOrdenadas = contexto.Keys.OrderByDescending(k => k.Length);
        foreach (var chave in chavesOrdenadas)
        {
            expressao = expressao.Replace($"{{{chave}}}", contexto[chave]?.ToString() ?? "null");
        }

        var partes = expressao.Split(" AND ", StringSplitOptions.TrimEntries);
        foreach (var parte in partes)
        {
            var orPartes = parte.Split(" OR ", StringSplitOptions.TrimEntries);
            bool orResult = false;
            foreach (var orParte in orPartes)
            {
                if (AvaliarExpressaoSimples(orParte.Trim(), contexto))
                {
                    orResult = true;
                    break;
                }
            }

            if (!orResult)
                return false;
        }

        return true;
    }

    private static bool AvaliarExpressaoSimples(string expressao, IReadOnlyDictionary<string, object> contexto)
    {
        string[] operadores = { ">=", "<=", "!=", "==", ">", "<" };
        foreach (var op in operadores)
        {
            var idx = expressao.IndexOf(op, StringComparison.Ordinal);
            if (idx < 0)
                continue;

            var left = expressao[..idx].Trim();
            var right = expressao[(idx + op.Length)..].Trim();

            var leftValue = ResolverValor(left, contexto);
            var rightValue = ResolverValor(right, contexto);

            if (leftValue is decimal leftDec && rightValue is decimal rightDec)
            {
                return op switch
                {
                    ">" => leftDec > rightDec,
                    "<" => leftDec < rightDec,
                    ">=" => leftDec >= rightDec,
                    "<=" => leftDec <= rightDec,
                    "==" => leftDec == rightDec,
                    "!=" => leftDec != rightDec,
                    _ => false,
                };
            }

            var leftStr = leftValue?.ToString() ?? string.Empty;
            var rightStr = rightValue?.ToString() ?? string.Empty;

            return op switch
            {
                "==" => string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase),
                _ => false,
            };
        }

        return false;
    }

    private static object? ResolverValor(string valor, IReadOnlyDictionary<string, object> contexto)
    {
        if (decimal.TryParse(valor, out var dec))
            return dec;

        if (contexto.TryGetValue(valor, out var ctxValue))
            return ctxValue;

        return valor;
    }
}
