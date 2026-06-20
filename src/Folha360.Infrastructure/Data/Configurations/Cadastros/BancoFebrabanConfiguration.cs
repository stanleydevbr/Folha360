using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade BancoFebraban — Bancos brasileiros conforme Febraban.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class BancoFebrabanConfiguration : IEntityTypeConfiguration<BancoFebraban>
{
    public void Configure(EntityTypeBuilder<BancoFebraban> entity)
    {
        entity.ToTable("bancos_febraban", "public");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(5);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Ativo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.Codigo).IsUnique();
    }
}
