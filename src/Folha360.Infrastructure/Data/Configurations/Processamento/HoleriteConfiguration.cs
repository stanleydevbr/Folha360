using Folha360.Processamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Processamento;

public class HoleriteConfiguration : IEntityTypeConfiguration<Holerite>
{
    public void Configure(EntityTypeBuilder<Holerite> entity)
    {
        entity.ToTable("holerite");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.ProcessamentoId).IsRequired();
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.MinioKey).IsRequired().HasColumnType("text");
        entity.Property(e => e.DataGeracao).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => e.ProcessamentoId);
        entity.HasIndex(e => e.FuncionarioId);
    }
}
