using Folha360.Fiscais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Fiscais;

public class GuiaRecolhimentoConfiguration : IEntityTypeConfiguration<GuiaRecolhimento>
{
    public void Configure(EntityTypeBuilder<GuiaRecolhimento> entity)
    {
        entity.ToTable("guia_recolhimento");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Periodo).IsRequired();
        entity.Property(e => e.ApuracaoFiscalId).IsRequired();
        entity.Property(e => e.TipoGuia).IsRequired().HasConversion<int>();
        entity.Property(e => e.Tributo).IsRequired().HasConversion<int>();
        entity.Property(e => e.CodigoReceita).IsRequired().HasMaxLength(10);
        entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.DataVencimento).IsRequired();
        entity.Property(e => e.MinioKey).HasColumnType("text");
        entity.Property(e => e.Status).IsRequired().HasConversion<int>();
        entity.Property(e => e.DataPagamento);
        entity.Property(e => e.ValorPago).HasColumnType("decimal(18,2)");
        entity.Property(e => e.ComprovanteMinioKey).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.EmpresaId, e.Periodo });
        entity.HasIndex(e => e.ApuracaoFiscalId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.DataVencimento);
    }
}
