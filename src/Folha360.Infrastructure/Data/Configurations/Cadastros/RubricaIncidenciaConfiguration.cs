using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaIncidenciaConfiguration : IEntityTypeConfiguration<RubricaIncidencia>
{
    public void Configure(EntityTypeBuilder<RubricaIncidencia> entity)
    {
        entity.ToTable("rubrica_incidencia");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.TipoIncidencia).IsRequired().HasMaxLength(30);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.RubricaId, e.TipoIncidencia }).IsUnique();

        entity.HasOne(e => e.Rubrica)
            .WithMany(r => r.Incidencias)
            .HasForeignKey(e => e.RubricaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
