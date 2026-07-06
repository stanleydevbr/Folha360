using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade NaturezaJuridica — Tabela 21 do e-Social.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class NaturezaJuridicaConfiguration : IEntityTypeConfiguration<NaturezaJuridica>
{
    public void Configure(EntityTypeBuilder<NaturezaJuridica> entity)
    {
        entity.ToTable("naturezas_juridicas", "public");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
        entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Ativo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.Codigo).IsUnique();
    }
}
