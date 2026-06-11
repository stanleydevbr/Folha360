using Folha360.Esocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Esocial;

public class FalhaEsocialConfiguration : IEntityTypeConfiguration<FalhaEsocial>
{
    public void Configure(EntityTypeBuilder<FalhaEsocial> entity)
    {
        entity.ToTable("falha_esocial");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EventoId).IsRequired();
        entity.Property(e => e.LoteId);
        entity.Property(e => e.TipoErro).IsRequired().HasConversion<string>().HasMaxLength(20);
        entity.Property(e => e.CodigoErro).HasMaxLength(20);
        entity.Property(e => e.MensagemErro).IsRequired().HasColumnType("text");
        entity.Property(e => e.XmlOriginal).HasColumnType("xml");
        entity.Property(e => e.Tentativas).IsRequired().HasDefaultValue(0);
        entity.Property(e => e.DataUltimaTentativa).IsRequired();
        entity.Property(e => e.ResolvidoEm);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => e.EventoId);
        entity.HasIndex(e => new { e.TipoErro, e.ResolvidoEm });
    }
}
