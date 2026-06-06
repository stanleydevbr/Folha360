using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class HorarioTrabalhoConfiguration : IEntityTypeConfiguration<HorarioTrabalho>
{
    public void Configure(EntityTypeBuilder<HorarioTrabalho> entity)
    {
        entity.ToTable("horario_trabalho");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.CargaHorariaDiaria).IsRequired();
        entity.Property(e => e.CargaHorariaSemanal).IsRequired();
        entity.Property(e => e.ToleranciaAtrasoMinutos).HasDefaultValue(0);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
    }
}
