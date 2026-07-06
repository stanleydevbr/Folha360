namespace Folha360.Domain.Validation;

/// <summary>
/// Representa um único erro de validação dentro de uma <see cref="Notification"/>.
/// </summary>
/// <param name="Code">Código categórico do erro (ex: "CPF_INVALIDO", "VALIDATION").</param>
/// <param name="Message">Descrição legível do erro.</param>
/// <param name="MemberName">Nome do membro/propriedade associada ao erro (opcional).</param>
public sealed record NotificationError(
    string Code,
    string Message,
    string? MemberName = null);
