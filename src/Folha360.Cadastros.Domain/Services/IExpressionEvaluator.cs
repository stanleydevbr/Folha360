namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Interface para avaliação de expressões matemáticas com sandbox.
/// Encapsula NCalc como detalhe de implementação.
/// </summary>
public interface IExpressionEvaluator
{
    /// <summary>
    /// Avalia uma expressão matemática com os parâmetros fornecidos.
    /// Deve implementar timeout e whitelist de funções.
    /// </summary>
    decimal Avaliar(string expressao, IReadOnlyDictionary<string, object> parametros);
}
