using Folha360.Esocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Esocial;

public class CertificadoDigitalConfiguration : IEntityTypeConfiguration<CertificadoDigital>
{
    public void Configure(EntityTypeBuilder<CertificadoDigital> entity)
    {
        entity.ToTable("certificado_digital");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Tipo).IsRequired().HasConversion<string>().HasMaxLength(3);
        entity.Property(e => e.ArquivoPfx).HasColumnType("bytea");
        entity.Property(e => e.CaminhoToken).HasMaxLength(500);
        entity.Property(e => e.SlotId);
        entity.Property(e => e.Emitente).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
        entity.Property(e => e.DataExpiracao).IsRequired();
        entity.Property(e => e.Ativo).IsRequired().HasDefaultValue(true);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Ativo });
        entity.HasIndex(e => e.DataExpiracao);
    }
}
