using Folha360.Esocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Esocial;

public class LoteEsocialConfiguration : IEntityTypeConfiguration<LoteEsocial>
{
    public void Configure(EntityTypeBuilder<LoteEsocial> entity)
    {
        entity.ToTable("lote_esocial");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.TipoAmbiente).IsRequired().HasConversion<string>().HasMaxLength(15);
        entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(25);
        entity.Property(e => e.ProtocoloEnvio).HasMaxLength(50);
        entity.Property(e => e.ReciboGovernoJson).HasColumnType("jsonb");
        entity.Property(e => e.QuantidadeEventos).IsRequired();
        entity.Property(e => e.DataEnvio);
        entity.Property(e => e.DataProcessamento);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Status });
        entity.HasIndex(e => e.ProtocoloEnvio).IsUnique().HasFilter("protocolo_envio IS NOT NULL");
    }
}
