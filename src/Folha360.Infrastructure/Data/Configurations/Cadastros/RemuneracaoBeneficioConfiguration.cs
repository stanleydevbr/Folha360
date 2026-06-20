using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade RemuneracaoBeneficio — remuneração e benefícios do funcionário (1:1).
/// Schema: tenant.
/// </summary>
public class RemuneracaoBeneficioConfiguration : IEntityTypeConfiguration<RemuneracaoBeneficio>
{
    public void Configure(EntityTypeBuilder<RemuneracaoBeneficio> entity)
    {
        entity.ToTable("remuneracoes_beneficios");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.SalarioBase).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.ValorHora).HasColumnType("decimal(18,4)");
        entity.Property(e => e.AdicionalInsalubridade).HasColumnType("decimal(18,2)");
        entity.Property(e => e.AdicionalPericulosidade).HasColumnType("decimal(18,2)");
        entity.Property(e => e.AdicionalNoturnoPercentual).HasColumnType("decimal(5,2)");
        entity.Property(e => e.AdicionalTransferenciaPercentual).HasColumnType("decimal(5,2)");
        entity.Property(e => e.ValeTransporte).IsRequired();
        entity.Property(e => e.ValeTransporteValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.ValeRefeicao).IsRequired();
        entity.Property(e => e.ValeRefeicaoValorDiario).HasColumnType("decimal(18,2)");
        entity.Property(e => e.PlanoSaude).IsRequired();
        entity.Property(e => e.PlanoSaudeValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.PlanoOdontologico).IsRequired();
        entity.Property(e => e.PlanoOdontologicoValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.SeguroVida).IsRequired();
        entity.Property(e => e.SeguroVidaValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.PrevidenciaPrivada).IsRequired();
        entity.Property(e => e.PrevidenciaPrivadaValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).IsUnique();
        entity.HasIndex(e => e.ConvenioId);
    }
}
