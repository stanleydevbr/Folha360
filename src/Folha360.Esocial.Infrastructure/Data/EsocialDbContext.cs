using Microsoft.EntityFrameworkCore;

namespace Folha360.Esocial.Infrastructure.Data;

public class EsocialDbContext : Folha360.Infrastructure.Data.Folha360DbContext
{
    public EsocialDbContext(DbContextOptions<Folha360.Infrastructure.Data.Folha360DbContext> options)
        : base(options)
    {
    }
}
