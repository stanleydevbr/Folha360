using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Infrastructure.Data;

public class Folha360DbContext : DbContext
{
    public Folha360DbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        // Tenant mapping
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenant");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SchemaName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.HasIndex(e => e.TenantId).IsUnique();
            entity.HasIndex(e => e.SchemaName).IsUnique();
        });

        // Usuario mapping
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Perfil).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.UltimoLogin);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // AuditLog mapping
        modelBuilder.Entity<AuditLogEntry>(entity =>
        {
            entity.ToTable("audit_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.SchemaName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RecordId).IsRequired();
            entity.Property(e => e.Action).IsRequired().HasMaxLength(10);
            entity.Property(e => e.OldData).HasColumnType("jsonb");
            entity.Property(e => e.NewData).HasColumnType("jsonb");
            entity.Property(e => e.ChangedBy).IsRequired();
            entity.Property(e => e.ChangedAt).IsRequired();
            entity.HasIndex(e => new { e.TableName, e.RecordId });
            entity.HasIndex(e => e.ChangedAt);
        });

        // Aplicar filtro global de schema do tenant para tabelas de negócio
        // (aplicado via interceptor em tempo de execução)
    }
}
