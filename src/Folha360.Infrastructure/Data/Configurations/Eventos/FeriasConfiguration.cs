using Folha360.Eventos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Eventos;

public class FeriasConfiguration : IEntityTypeConfiguration<Ferias>
{
    public void Configure(EntityTypeBuilder<Ferias> entity)
    {
        entity.ToTable("ferias");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.DataInicio).IsRequired();
        entity.Property(e => e.DiasGozo).IsRequired();
        entity.Property(e => e.PeriodoAquisitivoInicio).IsRequired();
        entity.Property(e => e.PeriodoAquisitivoFim).IsRequired();
        entity.Property(e => e.TipoFerias).IsRequired().HasConversion<int>();
        entity.Property(e => e.XmlContent).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
    }
}
