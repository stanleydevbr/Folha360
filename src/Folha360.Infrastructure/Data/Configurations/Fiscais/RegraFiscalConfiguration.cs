using Folha360.Fiscais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Fiscais;

public class RegraFiscalConfiguration : IEntityTypeConfiguration<RegraFiscal>
{
    public void Configure(EntityTypeBuilder<RegraFiscal> entity)
    {
        entity.ToTable("regra_fiscal");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Tributo).IsRequired().HasConversion<int>();
        entity.Property(e => e.Versao).IsRequired();
        entity.Property(e => e.VigenciaInicio).IsRequired();
        entity.Property(e => e.VigenciaFim);
        entity.Property(e => e.Parametros).IsRequired().HasColumnType("jsonb");
        entity.Property(e => e.CodigoReceita).IsRequired().HasMaxLength(10);
        entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.Tributo, e.Versao }).IsUnique();
        entity.HasIndex(e => new { e.Tributo, e.VigenciaInicio, e.VigenciaFim });
    }
}
