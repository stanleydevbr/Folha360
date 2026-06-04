using Folha360.Domain.Entities;

namespace Folha360.Domain.Abstractions;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
}
