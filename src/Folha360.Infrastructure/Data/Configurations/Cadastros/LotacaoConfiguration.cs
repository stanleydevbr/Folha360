using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class LotacaoConfiguration : IEntityTypeConfiguration<Lotacao>
{
    public void Configure(EntityTypeBuilder<Lotacao> entity)
    {
        entity.ToTable("lotacao");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
        entity.Property(e => e.TipoEsocial).HasMaxLength(10);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
    }
}
