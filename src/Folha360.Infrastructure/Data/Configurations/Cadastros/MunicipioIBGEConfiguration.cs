using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade MunicipioIBGE — Municípios brasileiros conforme IBGE.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class MunicipioIBGEConfiguration : IEntityTypeConfiguration<MunicipioIBGE>
{
    public void Configure(EntityTypeBuilder<MunicipioIBGE> entity)
    {
        entity.ToTable("municipios_ibge", "public");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.CodigoIbge).IsRequired().HasMaxLength(7);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Uf).IsRequired().HasMaxLength(2);
        entity.Property(e => e.CodigoUf).IsRequired().HasMaxLength(2);
        entity.Property(e => e.Ativo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.CodigoIbge).IsUnique();
        entity.HasIndex(e => e.Uf);
        entity.HasIndex(e => e.Nome);
    }
}
