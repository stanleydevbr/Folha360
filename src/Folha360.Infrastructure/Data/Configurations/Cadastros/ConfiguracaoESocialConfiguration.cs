using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade ConfiguracaoESocial — configurações de e-Social da empresa (1:1).
/// Schema: tenant. Não implementa ISoftDeletable.
/// </summary>
public class ConfiguracaoESocialConfiguration : IEntityTypeConfiguration<ConfiguracaoESocial>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoESocial> entity)
    {
        entity.ToTable("configuracoes_esocial");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Ambiente).IsRequired().HasMaxLength(20);
        entity.Property(e => e.CertificadoDigitalTipo).HasMaxLength(10);
        entity.Property(e => e.VersaoLayout).HasMaxLength(10);
        entity.Property(e => e.CodigoTransmissor).HasMaxLength(20);
        entity.Property(e => e.GrupoEsocial).HasMaxLength(10);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId).IsUnique();
    }
}
