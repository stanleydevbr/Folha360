namespace Folha360.Domain.Validation;

/// <summary>
/// Coletor de erros de validação (Notification Pattern — Martin Fowler).
/// Acumula múltiplos erros em vez de lançar exceções, permitindo que o
/// chamador receba todas as violações de uma só vez.
/// </summary>
public class Notification
{
    private readonly List<NotificationError> _errors = new();

    /// <summary>
    /// Lista de erros acumulados. Vazia se nenhum erro foi adicionado.
    /// </summary>
    public IReadOnlyList<NotificationError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// True se há pelo menos um erro acumulado.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// True se não há erros acumulados (inverso de <see cref="HasErrors"/>).
    /// </summary>
    public bool IsValid => !HasErrors;

    /// <summary>
    /// Adiciona um erro com código, mensagem e membro opcional.
    /// </summary>
    public void AddError(string code, string message, string? memberName = null)
        => _errors.Add(new NotificationError(code, message, memberName));

    /// <summary>
    /// Adiciona um erro com código padrão "VALIDATION".
    /// </summary>
    public void AddError(string message)
        => AddError("VALIDATION", message);

    /// <summary>
    /// Adiciona múltiplos erros de uma só vez.
    /// </summary>
    public void AddErrors(IEnumerable<NotificationError> errors)
        => _errors.AddRange(errors);

    /// <summary>
    /// Mescla os erros de outra <see cref="Notification"/> nesta instância.
    /// </summary>
    public void AddNotification(Notification other)
        => _errors.AddRange(other._errors);

    /// <summary>
    /// Concatena todas as mensagens de erro separadas por "; ".
    /// Útil para retrocompatibilidade com mensagens de exceção.
    /// </summary>
    public string ToErrorMessage()
        => string.Join("; ", _errors.Select(e => e.Message));
}
