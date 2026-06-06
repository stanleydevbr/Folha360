using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class SindicatoConfiguration : IEntityTypeConfiguration<Sindicato>
{
    public void Configure(EntityTypeBuilder<Sindicato> entity)
    {
        entity.ToTable("sindicato");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cnpj).HasMaxLength(14);
        entity.Property(e => e.Tipo).HasMaxLength(30);
        entity.Property(e => e.ContribuicaoSindicalPercentual).HasColumnType("decimal(5,2)");
        entity.Property(e => e.ContribuicaoAssistencialPercentual).HasColumnType("decimal(5,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
    }
}
