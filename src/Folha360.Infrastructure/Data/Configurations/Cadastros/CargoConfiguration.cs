using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> entity)
    {
        entity.ToTable("cargo");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cbo).IsRequired().HasMaxLength(6);
        entity.Property(e => e.Descricao).HasMaxLength(500);
        entity.Property(e => e.SalarioBaseMinimo).HasColumnType("decimal(18,2)");
        entity.Property(e => e.SalarioBaseMaximo).HasColumnType("decimal(18,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.Cbo);
    }
}
