using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly Folha360DbContext _context;

    public UsuarioRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}
