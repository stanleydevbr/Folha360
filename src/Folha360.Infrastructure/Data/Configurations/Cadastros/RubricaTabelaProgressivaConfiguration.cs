using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaTabelaProgressivaConfiguration : IEntityTypeConfiguration<RubricaTabelaProgressiva>
{
    public void Configure(EntityTypeBuilder<RubricaTabelaProgressiva> entity)
    {
        entity.ToTable("rubrica_tabela_progressiva");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FaixaDe).IsRequired().HasColumnType("numeric(18,2)");
        entity.Property(e => e.FaixaAte).HasColumnType("numeric(18,2)");
        entity.Property(e => e.Aliquota).IsRequired().HasColumnType("numeric(7,4)");
        entity.Property(e => e.Deducao).IsRequired().HasColumnType("numeric(18,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.RubricaId, e.AnoVigencia, e.Ordem });

        entity.HasOne(e => e.Rubrica)
            .WithMany(r => r.TabelasProgressivas)
            .HasForeignKey(e => e.RubricaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
