using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class RelatorioArquivoConfiguration : IEntityTypeConfiguration<RelatorioArquivo>
{
    public void Configure(EntityTypeBuilder<RelatorioArquivo> builder)
    {
        builder.ToTable("relatorio_arquivo");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(e => e.TipoRelatorio).HasColumnName("tipo_relatorio").HasConversion<int>().IsRequired();
        builder.Property(e => e.Periodo).HasColumnName("periodo").HasMaxLength(7).IsRequired();
        builder.Property(e => e.Formato).HasColumnName("formato").HasConversion<int>().IsRequired();
        builder.Property(e => e.Bucket).HasColumnName("bucket").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Chave).HasColumnName("chave").HasMaxLength(500).IsRequired();
        builder.Property(e => e.TamanhoBytes).HasColumnName("tamanho_bytes").IsRequired();
        builder.Property(e => e.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(e => new { e.EmpresaId, e.Periodo });
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
