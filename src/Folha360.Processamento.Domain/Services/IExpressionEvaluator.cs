namespace Folha360.Processamento.Domain.Services;

public interface IExpressionEvaluator
{
    decimal Avaliar(string expressao, IReadOnlyDictionary<string, object> parametros);
}
