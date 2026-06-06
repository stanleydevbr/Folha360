using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Base;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> entity)
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
    }
}
