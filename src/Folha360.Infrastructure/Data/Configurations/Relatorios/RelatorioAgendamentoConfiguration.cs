using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class RelatorioAgendamentoConfiguration : IEntityTypeConfiguration<RelatorioAgendamento>
{
    public void Configure(EntityTypeBuilder<RelatorioAgendamento> builder)
    {
        builder.ToTable("relatorio_agendamento");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(e => e.TipoRelatorio).HasColumnName("tipo_relatorio").HasConversion<int>().IsRequired();
        builder.Property(e => e.Formato).HasColumnName("formato").HasConversion<int>().IsRequired();
        builder.Property(e => e.Recorrencia).HasColumnName("recorrencia").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Destinatarios).HasColumnName("destinatarios").HasColumnType("jsonb").IsRequired();
        builder.Property(e => e.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(e => e.CriadoEm).HasColumnName("criado_em").IsRequired();
        builder.Property(e => e.AtualizadoEm).HasColumnName("atualizado_em");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(e => new { e.EmpresaId, e.Ativo });
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
