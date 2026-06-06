using Folha360.Eventos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Eventos;

public class AlteracaoContratualConfiguration : IEntityTypeConfiguration<AlteracaoContratual>
{
    public void Configure(EntityTypeBuilder<AlteracaoContratual> entity)
    {
        entity.ToTable("alteracao_contratual");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.DataAlteracao).IsRequired();
        entity.Property(e => e.CamposAlterados).HasColumnType("jsonb");
        entity.Property(e => e.ValorAnterior).HasColumnType("jsonb");
        entity.Property(e => e.ValorNovo).HasColumnType("jsonb");
        entity.Property(e => e.XmlContent).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
    }
}
