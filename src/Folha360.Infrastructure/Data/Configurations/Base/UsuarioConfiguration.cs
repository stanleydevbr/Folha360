using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Base;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entity)
    {
        entity.ToTable("usuario");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(500);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Perfil).IsRequired();
        entity.Property(e => e.Status).IsRequired();
        entity.Property(e => e.UltimoLogin);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.HasIndex(e => e.Email).IsUnique();
    }
}
