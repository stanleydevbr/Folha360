using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade InfoESocialFuncionario — informações complementares para e-Social (1:1).
/// Schema: tenant. Não implementa ISoftDeletable.
/// </summary>
public class InfoESocialFuncionarioConfiguration : IEntityTypeConfiguration<InfoESocialFuncionario>
{
    public void Configure(EntityTypeBuilder<InfoESocialFuncionario> entity)
    {
        entity.ToTable("infos_esocial_funcionario");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.IndicadorDeficiencia).IsRequired();
        entity.Property(e => e.TipoDeficiencia).HasMaxLength(20);
        entity.Property(e => e.Reservista).IsRequired();
        entity.Property(e => e.PrimeiroEmprego).IsRequired();
        entity.Property(e => e.TrabalhadorAposentado).IsRequired();
        entity.Property(e => e.RegistroProfissional).HasMaxLength(50);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).IsUnique();
    }
}
