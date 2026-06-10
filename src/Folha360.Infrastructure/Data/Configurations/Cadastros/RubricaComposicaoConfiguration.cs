using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaComposicaoConfiguration : IEntityTypeConfiguration<RubricaComposicao>
{
    public void Configure(EntityTypeBuilder<RubricaComposicao> entity)
    {
        entity.ToTable("rubrica_composicao");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Operador).IsRequired().HasMaxLength(5);
        entity.Property(e => e.PercentualComposicao).HasColumnType("numeric(7,4)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.RubricaPrincipalId, e.RubricaComponenteId }).IsUnique();
        entity.HasIndex(e => e.RubricaComponenteId);

        entity.HasOne(e => e.RubricaPrincipal)
            .WithMany(r => r.Composicoes)
            .HasForeignKey(e => e.RubricaPrincipalId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.RubricaComponente)
            .WithMany()
            .HasForeignKey(e => e.RubricaComponenteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
