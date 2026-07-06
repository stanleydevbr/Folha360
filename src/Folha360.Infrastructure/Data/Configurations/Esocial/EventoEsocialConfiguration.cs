using Folha360.Esocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Esocial;

public class EventoEsocialConfiguration : IEntityTypeConfiguration<EventoEsocial>
{
    public void Configure(EntityTypeBuilder<EventoEsocial> entity)
    {
        entity.ToTable("evento_esocial");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.FuncionarioId);
        entity.Property(e => e.TipoEvento).IsRequired().HasConversion<string>().HasMaxLength(10);
        entity.Property(e => e.XmlConteudo).IsRequired().HasColumnType("xml");
        entity.Property(e => e.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        entity.Property(e => e.LoteId);
        entity.Property(e => e.IdEvento).IsRequired().HasMaxLength(50);
        entity.Property(e => e.CertificadoId);
        entity.Property(e => e.HashAssinatura).HasMaxLength(128);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.Property(e => e.ProcessadoEm);

        entity.HasIndex(e => new { e.EmpresaId, e.TipoEvento, e.Status });
        entity.HasIndex(e => e.LoteId);
        entity.HasIndex(e => e.IdEvento).IsUnique();
    }
}
