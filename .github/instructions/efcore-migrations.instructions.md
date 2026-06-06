---
description: "Use when: creating EF Core migrations, modifying DbContext, adding new entities, adding a new module (F03, F04...), configuring IEntityTypeConfiguration, setting up snake_case naming, or working with database schema changes in the Folha360 modular monolith."
applyTo: ["**/Data/*DbContext*.cs", "**/Data/Configurations/**", "**/Migrations/**", "**/*.csproj"]
---

# EF Core Migration Guidelines — Folha360

O Folha360 adota a arquitetura de **Migrations Assembly Centralizado**.
Há um ÚNICO assembly dono das migrations (`Folha360.Infrastructure`) e um ÚNICO DbContext que define o modelo canônico (`Folha360DbContext`).

---

## 1. Arquitetura: Migrations Assembly Centralizado

```
Folha360.Infrastructure/              ← DONO das migrations
├── Data/
│   ├── Configurations/
│   │   ├── Base/                     ← TenantConfiguration, UsuarioConfiguration, AuditLogConfiguration
│   │   ├── Cadastros/                ← EmpresaConfiguration, FuncionarioConfiguration, ... (10 F02)
│   │   └── [NovoModulo]/             ← Configurações do novo módulo (F03, F04...)
│   ├── Folha360DbContext.cs          ← Modelo CANÔNICO (TODAS as entidades de TODOS os módulos)
│   ├── Folha360DbContextFactory.cs   ← Design-time factory
│   ├── EncryptionConverter.cs       ← ValueConverter compartilhado
│   └── Migrations/                   ← ÚNICO local de migrations
│
Folha360.[Modulo].Infrastructure/
├── Data/
│   └── [Modulo]DbContext.cs          ← Runtime-only (herda do modelo canônico)
```

## 2. Modelo Canônico: Folha360DbContext

O `Folha360DbContext` contém **TODAS** as entidades de **TODOS** os módulos:

```csharp
public class Folha360DbContext : DbContext
{
    // Tabelas base
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    // Módulo F02 — Cadastros
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    // ... demais entidades F02

    // Módulo F03 — (futuro)
    // public DbSet<Evento> Eventos => Set<Evento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        // Configurações via IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        // Módulo F02
        modelBuilder.ApplyConfiguration(new EmpresaConfiguration());
        modelBuilder.ApplyConfiguration(new FuncionarioConfiguration());
        // ...

        // Módulo F03 (futuro)
        // modelBuilder.ApplyConfiguration(new EventoConfiguration());

        // Soft delete query filter global (NUNCA remover)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var nullCheck = Expression.Equal(property, Expression.Constant(null, typeof(DateTime?)));
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(nullCheck, parameter));
            }
        }
    }
}
```

## 3. DbContext de Módulo: Runtime-Only

Cada módulo tem seu próprio DbContext que herda do modelo canônico, mas **NÃO adiciona configurações** (já estão no canônico) e **NÃO tem migrations**:

```csharp
public class CadastrosDbContext : Folha360DbContext
{
    public CadastrosDbContext(DbContextOptions<Folha360DbContext> options) : base(options) { }

    // DbSets tipados com 'new' para queries específicas do módulo
    public new DbSet<Empresa> Empresas => Set<Empresa>();
    public new DbSet<Funcionario> Funcionarios => Set<Funcionario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Delega TUDO para o canônico
    }

    // Soft delete interceptor (opcional — já está no canônico, mas pode ser reforçado aqui)
    private void InterceptSoftDelete() { /* ... */ }
}
```

## 4. Snake Case Naming Convention

**SEMPRE** usar `UseSnakeCaseNamingConvention()` em TODOS os DbContexts:

```csharp
// No DI (ServiceCollectionExtensions)
services.AddDbContextFactory<Folha360DbContext>(options =>
    options.UseSnakeCaseNamingConvention()
           .UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(3)));

// Na factory de design-time
optionsBuilder.UseSnakeCaseNamingConvention().UseNpgsql(connectionString);
```

Pacote necessário: `EFCore.NamingConventions` versão compatível com EF Core 10.

## 5. Program.cs: UMA ÚNICA chamada MigrateAsync

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<Folha360DbContext>();
    await context.Database.MigrateAsync(); // ÚNICA chamada — resolve TUDO
    await SeedData.InitializeAsync(scope.ServiceProvider);
}
```

**NUNCA** chamar `MigrateAsync()` em outro DbContext. **NUNCA** usar `EnsureCreatedAsync()`.

## 6. Gerar Migrations

As migrations são SEMPRE geradas a partir do projeto `Folha360.Infrastructure`:

```bash
dotnet ef migrations add <NomeDaMigration> \
    --project src/Folha360.Infrastructure/Folha360.Infrastructure.csproj \
    --startup-project src/Folha360.WebApi/Folha360.WebApi.csproj \
    --context Folha360DbContext
```

## 7. Checklist para Novo Módulo (F03, F04...)

- [ ] Criar `IEntityTypeConfiguration<T>` para cada nova entidade em `src/Folha360.Infrastructure/Data/Configurations/[NovoModulo]/`
- [ ] Adicionar `DbSet<T>` no `Folha360DbContext`
- [ ] Adicionar `modelBuilder.ApplyConfiguration(new ...)` no `OnModelCreating`
- [ ] Adicionar referência ao projeto Domain do módulo no `Folha360.Infrastructure.csproj` (se necessário)
- [ ] Criar `[Modulo]DbContext` runtime-only herdando de `Folha360DbContext` (com `new DbSet<T>`)
- [ ] Registrar `[Modulo]DbContext` no DI com `UseSnakeCaseNamingConvention()`
- [ ] Gerar migration: `dotnet ef migrations add Add[Modulo]Entities --project src/Folha360.Infrastructure`
- [ ] `dotnet build` completo passa
- [ ] Extensões PostgreSQL necessárias (ex: `pg_trgm`) adicionadas ao `docker/postgres/init.sh`

## 8. Extensões PostgreSQL

O `docker/postgres/init.sh` DEVE conter todas as extensões usadas:

```sql
CREATE EXTENSION IF NOT EXISTS vector;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS pg_trgm;       -- índices GIN com trigram
-- Adicionar novas extensões aqui conforme necessário
```

## 9. NÃO FAZER

- ❌ NÃO criar migrations em projetos de módulo (`Cadastros.Infrastructure`, `Eventos.Infrastructure`)
- ❌ NÃO usar `EnsureCreatedAsync()` — sempre usar `MigrateAsync()`
- ❌ NÃO suprimir `PendingModelChangesWarning` — não é mais necessário
- ❌ NÃO criar `IDesignTimeDbContextFactory` para DbContexts de módulo
- ❌ NÃO usar `new DbContextOptionsBuilder<CadastrosDbContext>` — usar sempre `Folha360DbContext`
- ❌ NÃO esquecer de adicionar extensões PostgreSQL ao `init.sh`
