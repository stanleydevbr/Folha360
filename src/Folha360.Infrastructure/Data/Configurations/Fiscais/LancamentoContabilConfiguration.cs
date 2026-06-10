using Folha360.Fiscais.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Fiscais;

public class LancamentoContabilConfiguration : IEntityTypeConfiguration<LancamentoContabil>
{
    public void Configure(EntityTypeBuilder<LancamentoContabil> entity)
    {
        entity.ToTable("lancamento_contabil");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Periodo).IsRequired();
        entity.Property(e => e.ApuracaoFiscalId).IsRequired();
        entity.Property(e => e.Data).IsRequired();
        entity.Property(e => e.ContaDebito).IsRequired().HasMaxLength(50);
        entity.Property(e => e.ContaCredito).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Historico).HasColumnType("text");
        entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.Tributo).IsRequired().HasConversion<int>();
        entity.Property(e => e.Formato).IsRequired().HasConversion<int>();
        entity.Property(e => e.MinioKey).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.EmpresaId, e.Periodo });
        entity.HasIndex(e => e.ApuracaoFiscalId);
    }
}
