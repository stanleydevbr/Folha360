using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaConfiguration : IEntityTypeConfiguration<Rubrica>
{
    public void Configure(EntityTypeBuilder<Rubrica> entity)
    {
        entity.ToTable("rubrica");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
        entity.Property(e => e.DescricaoAbreviada).HasMaxLength(50);
        entity.Property(e => e.Natureza).IsRequired().HasMaxLength(20);
        entity.Property(e => e.TipoEsocial).HasMaxLength(10);
        entity.Property(e => e.TipoCalculo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.FormulaCalculo).HasColumnType("text");
        entity.Property(e => e.ValorFixo).HasColumnType("numeric(18,4)");
        entity.Property(e => e.Percentual).HasColumnType("numeric(7,4)");
        entity.Property(e => e.TetoMaximo).HasColumnType("numeric(18,2)");
        entity.Property(e => e.PisoMinimo).HasColumnType("numeric(18,2)");
        entity.Property(e => e.Observacao).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.Natureza);
        entity.HasIndex(e => e.TipoEsocial);
        entity.HasIndex(e => e.GrupoRubricaId);

        entity.HasOne(e => e.GrupoRubrica)
            .WithMany(g => g.Rubricas)
            .HasForeignKey(e => e.GrupoRubricaId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(e => e.RubricaBase)
            .WithMany()
            .HasForeignKey(e => e.RubricaBaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
