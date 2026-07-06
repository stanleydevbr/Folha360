using Folha360.Domain.Abstractions;
using Folha360.Domain.Validation;

namespace Folha360.Domain;

public abstract class BaseEntity : ISoftDeletable, INotificavel
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Coletor de notificações de validação da entidade.
    /// </summary>
    public Notification Notification { get; } = new();

    /// <summary>
    /// True se a entidade está em estado válido (sem notificações de erro).
    /// </summary>
    public bool IsValid => Notification.IsValid;

    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid id, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Executa as validações de negócio da entidade.
    /// Subclasses devem sobrescrever este método para adicionar suas
    /// próprias validações usando <see cref="Notification.AddError"/>.
    /// </summary>
    public virtual void Validate()
    {
        // Subclasses sobrescrevem para adicionar validações específicas
    }

    /// <summary>
    /// Remove caracteres não-dígito de uma string (ex.: formatação de CNPJ/CPF).
    /// Retorna null se a entrada for null.
    /// </summary>
    protected static string? StripNonDigits(string? value)
        => value is null ? null : new string(value.Where(char.IsDigit).ToArray());
}
