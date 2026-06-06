using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class ConvenioConfiguration : IEntityTypeConfiguration<Convenio>
{
    public void Configure(EntityTypeBuilder<Convenio> entity)
    {
        entity.ToTable("convenio");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Operadora).HasMaxLength(100);
        entity.Property(e => e.ValorMensal).HasColumnType("decimal(18,2)");
        entity.Property(e => e.PercentualEmpresa).HasColumnType("decimal(5,2)");
        entity.Property(e => e.PercentualFuncionario).HasColumnType("decimal(5,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
    }
}
