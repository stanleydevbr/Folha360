using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> entity)
    {
        entity.ToTable("documento");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Numero).IsRequired().HasConversion<EncryptionConverter>();
        entity.Property(e => e.OrgaoEmissor).HasMaxLength(50);
        entity.Property(e => e.UfEmissor).HasMaxLength(2);
        entity.Property(e => e.ArquivoPath).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => new { e.FuncionarioId, e.Tipo }).IsUnique().HasFilter("deleted_at IS NULL");
    }
}
