using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class GrupoRubricaConfiguration : IEntityTypeConfiguration<GrupoRubrica>
{
    public void Configure(EntityTypeBuilder<GrupoRubrica> entity)
    {
        entity.ToTable("grupo_rubrica");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Natureza).IsRequired().HasMaxLength(20);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();

        entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique();
    }
}
