using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class RubricaHistoricoConfiguration : IEntityTypeConfiguration<RubricaHistorico>
{
    public void Configure(EntityTypeBuilder<RubricaHistorico> entity)
    {
        entity.ToTable("rubrica_historico");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.DadosAnteriores).HasColumnType("jsonb");
        entity.Property(e => e.DadosNovos).IsRequired().HasColumnType("jsonb");
        entity.Property(e => e.Motivo).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.RubricaId, e.CreatedAt }).IsDescending(false, true);

        entity.HasOne(e => e.Rubrica)
            .WithMany(r => r.Historico)
            .HasForeignKey(e => e.RubricaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
