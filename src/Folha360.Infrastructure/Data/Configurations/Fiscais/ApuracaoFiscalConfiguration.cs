using Folha360.Fiscais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Fiscais;

public class ApuracaoFiscalConfiguration : IEntityTypeConfiguration<ApuracaoFiscal>
{
    public void Configure(EntityTypeBuilder<ApuracaoFiscal> entity)
    {
        entity.ToTable("apuracao_fiscal");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Periodo).IsRequired();
        entity.Property(e => e.ProcessamentoId).IsRequired();
        entity.Property(e => e.Tributo).IsRequired().HasConversion<int>();
        entity.Property(e => e.BaseCalculo).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.Aliquota).IsRequired().HasColumnType("decimal(7,4)");
        entity.Property(e => e.ValorDevido).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.ValorPago).HasColumnType("decimal(18,2)");
        entity.Property(e => e.DataVencimento).IsRequired();
        entity.Property(e => e.RegraFiscalId);
        entity.Property(e => e.Status).IsRequired().HasConversion<int>();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Periodo, e.Tributo, e.ProcessamentoId })
            .IsUnique()
            .HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => new { e.EmpresaId, e.Periodo });
        entity.HasIndex(e => e.ProcessamentoId);
    }
}
