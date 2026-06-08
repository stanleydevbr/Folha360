using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaProcessoConfiguration : IEntityTypeConfiguration<RubricaProcesso>
{
    public void Configure(EntityTypeBuilder<RubricaProcesso> entity)
    {
        entity.ToTable("rubrica_processo");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.RubricaId, e.ProcessoAdministrativoId }).IsUnique();

        entity.HasOne(e => e.Rubrica)
            .WithMany()
            .HasForeignKey(e => e.RubricaId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.ProcessoAdministrativo)
            .WithMany(p => p.RubricasProcesso)
            .HasForeignKey(e => e.ProcessoAdministrativoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
