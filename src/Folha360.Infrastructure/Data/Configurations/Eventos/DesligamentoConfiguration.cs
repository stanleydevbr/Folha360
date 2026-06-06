using Folha360.Eventos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Eventos;

public class DesligamentoConfiguration : IEntityTypeConfiguration<Desligamento>
{
    public void Configure(EntityTypeBuilder<Desligamento> entity)
    {
        entity.ToTable("desligamento");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.DataDesligamento).IsRequired();
        entity.Property(e => e.MotivoDesligamento).IsRequired().HasConversion<int>();
        entity.Property(e => e.VerbasRescisorias).HasColumnType("jsonb");
        entity.Property(e => e.XmlContent).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
    }
}
