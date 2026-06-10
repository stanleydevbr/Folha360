using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class RelatorioExecucaoConfiguration : IEntityTypeConfiguration<RelatorioExecucao>
{
    public void Configure(EntityTypeBuilder<RelatorioExecucao> builder)
    {
        builder.ToTable("relatorio_execucao");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.AgendamentoId).HasColumnName("agendamento_id").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(e => e.IniciadoEm).HasColumnName("iniciado_em").IsRequired();
        builder.Property(e => e.ConcluidoEm).HasColumnName("concluido_em");
        builder.Property(e => e.LinkArquivo).HasColumnName("link_arquivo").HasMaxLength(2000);
        builder.Property(e => e.LogErros).HasColumnName("log_erros").HasColumnType("text");

        builder.HasIndex(e => e.AgendamentoId);
    }
}
