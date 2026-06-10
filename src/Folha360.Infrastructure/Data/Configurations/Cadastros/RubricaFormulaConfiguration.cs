using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaFormulaConfiguration : IEntityTypeConfiguration<RubricaFormula>
{
    public void Configure(EntityTypeBuilder<RubricaFormula> entity)
    {
        entity.ToTable("rubrica_formula");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Expressao).IsRequired().HasColumnType("text");
        entity.Property(e => e.Parametros).HasColumnType("jsonb");
        entity.Property(e => e.DescricaoFormal).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => e.RubricaId).IsUnique();

        entity.HasOne(e => e.Rubrica)
            .WithOne(r => r.Formula)
            .HasForeignKey<RubricaFormula>(e => e.RubricaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
