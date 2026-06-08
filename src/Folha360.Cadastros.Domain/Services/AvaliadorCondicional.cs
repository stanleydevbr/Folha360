namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Avaliador de condições parametrizáveis para rubricas do tipo CONDICIONAL.
/// Suporta operadores: >, <, >=, <=, ==, !=, AND, OR.
/// </summary>
public class AvaliadorCondicional
{
    /// <summary>
    /// Avalia uma condição simples com operadores relacionais.
    /// Exemplo: "SALARIO_BASE > 3000 AND TIPO_CONTRATO == CLT"
    /// </summary>
    public bool Avaliar(string condicao, IReadOnlyDictionary<string, object> contexto)
    {
        if (string.IsNullOrWhiteSpace(condicao))
            return true;

        // Substituir placeholders por valores do contexto
        var expressao = condicao;
        foreach (var kv in contexto)
            expressao = expressao.Replace($"{{{kv.Key}}}", kv.Value?.ToString() ?? "null");

        // Suporte a operadores simples: valor1 > valor2, valor1 == valor2, etc.
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
            {
                return false;
            }
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
            {
                continue;
            }

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
                    _ => false
                };
            }

            var leftStr = leftValue?.ToString() ?? string.Empty;
            var rightStr = rightValue?.ToString() ?? string.Empty;

            return op switch
            {
                "==" => string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(leftStr, rightStr, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        return false;
    }

    private static object? ResolverValor(string valor, IReadOnlyDictionary<string, object> contexto)
    {
        if (decimal.TryParse(valor, out var dec))
        {
            return dec;
        }

        if (bool.TryParse(valor, out var b))
        {
            return b;
        }

        return valor.Trim('"', '\'');
    }
}
