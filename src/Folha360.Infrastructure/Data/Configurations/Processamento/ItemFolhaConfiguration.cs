using Folha360.Processamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Processamento;

public class ItemFolhaConfiguration : IEntityTypeConfiguration<ItemFolha>
{
    public void Configure(EntityTypeBuilder<ItemFolha> entity)
    {
        entity.ToTable("item_folha");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.ProcessamentoId).IsRequired();
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.RubricaId).IsRequired();
        entity.Property(e => e.Fase).IsRequired().HasConversion<int>();
        entity.Property(e => e.BaseCalculo).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.FormulaAplicada).HasColumnType("text");
        entity.Property(e => e.Ordem).IsRequired();
        entity.Property(e => e.DataCalculo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.ProcessamentoId, e.FuncionarioId });
        entity.HasIndex(e => e.RubricaId);
    }
}
