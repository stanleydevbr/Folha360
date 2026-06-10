using Folha360.Processamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Processamento;

public class CadeiaFechamentoConfiguration : IEntityTypeConfiguration<CadeiaFechamento>
{
    public void Configure(EntityTypeBuilder<CadeiaFechamento> entity)
    {
        entity.ToTable("cadeia_fechamento");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Periodo).IsRequired();
        entity.Property(e => e.ProcessamentoId).IsRequired();
        entity.Property(e => e.Etapa).IsRequired().HasConversion<int>();
        entity.Property(e => e.Status).IsRequired().HasConversion<int>();
        entity.Property(e => e.Versao).IsRequired().HasDefaultValue(1);
        entity.Property(e => e.HistoricoVersoes).IsRequired().HasColumnType("jsonb").HasDefaultValue("[]");
        entity.Property(e => e.DataInicio);
        entity.Property(e => e.DataFim);
        entity.Property(e => e.Erro).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Periodo }).IsUnique();
        entity.HasIndex(e => e.Status);
    }
}
