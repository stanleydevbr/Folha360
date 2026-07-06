namespace Folha360.Domain.Validation;

/// <summary>
/// Builder base para value objects imutáveis que precisam validar antes de construir.
/// O builder coleta notificações de validação e, se o estado for válido,
/// produz o value object através de um construtor privado.
/// </summary>
/// <typeparam name="T">Tipo do value object a ser construído.</typeparam>
public abstract class NotifiableValueObject<T>
    where T : class
{
    /// <summary>
    /// Coletor de notificações disponível para as subclasses durante a validação
    /// e para o chamador após <see cref="Build()"/> retornar null.
    /// </summary>
    public Notification Notification { get; } = new();

    /// <summary>
    /// True se o builder não possui notificações de erro.
    /// </summary>
    public bool IsValid => !Notification.HasErrors;

    /// <summary>
    /// Tenta construir o value object. Retorna null se houver erros de validação.
    /// As notificações ficam disponíveis em <see cref="Notification"/>.
    /// </summary>
    public abstract T? Build();
}
