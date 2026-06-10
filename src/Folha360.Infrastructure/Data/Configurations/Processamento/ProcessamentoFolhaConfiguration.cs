using Folha360.Processamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Processamento;

public class ProcessamentoFolhaConfiguration : IEntityTypeConfiguration<ProcessamentoFolha>
{
    public void Configure(EntityTypeBuilder<ProcessamentoFolha> entity)
    {
        entity.ToTable("processamento_folha");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.Periodo).IsRequired();
        entity.Property(e => e.TipoCalculo).IsRequired().HasConversion<int>();
        entity.Property(e => e.Status).IsRequired().HasConversion<int>();
        entity.Property(e => e.Versao).IsRequired().HasDefaultValue(1);
        entity.Property(e => e.ProcessamentoOriginalId);
        entity.Property(e => e.ReabertoPor).HasMaxLength(255);
        entity.Property(e => e.ReabertoEm);
        entity.Property(e => e.MotivoReabertura).HasColumnType("text");
        entity.Property(e => e.TotalFuncionarios).IsRequired().HasDefaultValue(0);
        entity.Property(e => e.FuncionariosProcessados).IsRequired().HasDefaultValue(0);
        entity.Property(e => e.FuncionariosComErro).IsRequired().HasDefaultValue(0);
        entity.Property(e => e.TotalVencimentos).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
        entity.Property(e => e.TotalDescontos).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
        entity.Property(e => e.TotalLiquido).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
        entity.Property(e => e.TotalFgts).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0);
        entity.Property(e => e.DataInicio);
        entity.Property(e => e.DataFim);
        entity.Property(e => e.Erro).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => new { e.EmpresaId, e.Periodo, e.TipoCalculo, e.Versao }).IsUnique();
        entity.HasIndex(e => new { e.EmpresaId, e.Periodo });
        entity.HasIndex(e => e.Status);
    }
}
