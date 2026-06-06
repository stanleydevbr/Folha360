using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Base;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> entity)
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
    }
}
