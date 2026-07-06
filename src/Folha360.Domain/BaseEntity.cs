using Folha360.Domain.Abstractions;

namespace Folha360.Domain;

public abstract class BaseEntity : ISoftDeletable
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

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
    /// Remove caracteres não-dígito de uma string (ex.: formatação de CNPJ/CPF).
    /// Retorna null se a entrada for null.
    /// </summary>
    protected static string? StripNonDigits(string? value)
        => value is null ? null : new string(value.Where(char.IsDigit).ToArray());
}
