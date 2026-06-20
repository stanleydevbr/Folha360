using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade CboOcupacao — Classificação Brasileira de Ocupações (MTb).
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class CboOcupacaoConfiguration : IEntityTypeConfiguration<CboOcupacao>
{
    public void Configure(EntityTypeBuilder<CboOcupacao> entity)
    {
        entity.ToTable("cbos", "public");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(6);
        entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Ativo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.Codigo).IsUnique();
    }
}
