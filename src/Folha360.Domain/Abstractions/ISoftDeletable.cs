namespace Folha360.Domain.Abstractions;

/// <summary>
/// Interface para entidades que suportam soft delete (exclusão lógica).
/// Entidades que implementam esta interface terão o query filter global
/// aplicado automaticamente pelo DbContext (deleted_at IS NULL).
/// </summary>
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
