namespace Folha360.Domain.Validation;

/// <summary>
/// Interface para entidades e value objects que produzem notificações de validação.
/// Implementar esta interface permite que o domínio acumule erros em vez de
/// lançar exceções para validações de negócio.
/// </summary>
public interface INotificavel
{
    /// <summary>
    /// Coletor de erros de validação da entidade/value object.
    /// </summary>
    Notification Notification { get; }

    /// <summary>
    /// True se a entidade/value object está em estado válido (sem notificações).
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Executa as validações de negócio e preenche <see cref="Notification"/>
    /// com os erros encontrados.
    /// </summary>
    void Validate();
}
