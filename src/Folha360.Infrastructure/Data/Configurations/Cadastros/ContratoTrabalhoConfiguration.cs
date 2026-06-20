using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade ContratoTrabalho — vínculo empregatício detalhado (1:1 com Funcionario).
/// Schema: tenant.
/// </summary>
public class ContratoTrabalhoConfiguration : IEntityTypeConfiguration<ContratoTrabalho>
{
    public void Configure(EntityTypeBuilder<ContratoTrabalho> entity)
    {
        entity.ToTable("contratos_trabalho");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.DataAdmissao).IsRequired();
        entity.Property(e => e.TipoContrato).IsRequired().HasMaxLength(30);
        entity.Property(e => e.SalarioBase).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.TipoSalario).IsRequired().HasMaxLength(20);
        entity.Property(e => e.TipoAdmissao).HasMaxLength(10);
        entity.Property(e => e.CargaHorariaSemanal);
        entity.Property(e => e.CategoriaTrabalhador).HasMaxLength(10);
        entity.Property(e => e.IndicativoAdmissao).HasMaxLength(10);
        entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).IsUnique();
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.LotacaoId);
        entity.HasIndex(e => e.CargoId);
        entity.HasIndex(e => e.HorarioTrabalhoId);
        entity.HasIndex(e => e.SindicatoId);
        entity.HasIndex(e => e.Status);
    }
}
