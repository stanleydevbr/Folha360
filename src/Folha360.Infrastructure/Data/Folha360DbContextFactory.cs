using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Folha360.Infrastructure.Data;

public class Folha360DbContextFactory : IDesignTimeDbContextFactory<Folha360DbContext>
{
    public Folha360DbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<Folha360DbContext>();
        var connectionString = args.Length > 0
            ? args[0]
            : "Host=localhost;Port=5432;Database=folha360;Username=folha360_user;Password=Folha360@Dev";

        optionsBuilder.UseNpgsql(connectionString);

        return new Folha360DbContext(optionsBuilder.Options);
    }
}
