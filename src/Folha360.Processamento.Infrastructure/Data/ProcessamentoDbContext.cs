using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Processamento.Infrastructure.Data;

public class ProcessamentoDbContext : Folha360DbContext
{
    public ProcessamentoDbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }
}
