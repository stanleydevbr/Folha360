using Microsoft.EntityFrameworkCore;

namespace Folha360.Fiscais.Infrastructure.Data;

/// <summary>
/// DbContext runtime-only para o módulo F05 — Obrigações Fiscais.
/// Delega toda configuração de modelo para o Folha360DbContext canônico.
/// </summary>
public class FiscaisDbContext : Folha360.Infrastructure.Data.Folha360DbContext
{
    public FiscaisDbContext(DbContextOptions<Folha360.Infrastructure.Data.Folha360DbContext> options)
        : base(options)
    {
    }
}
