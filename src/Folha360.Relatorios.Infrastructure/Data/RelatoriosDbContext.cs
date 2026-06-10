namespace Folha360.Relatorios.Infrastructure.Data;

public class RelatoriosDbContext : Folha360DbContext
{
    public RelatoriosDbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }
}
