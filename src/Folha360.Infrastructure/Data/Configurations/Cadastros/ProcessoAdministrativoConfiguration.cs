using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class ProcessoAdministrativoConfiguration : IEntityTypeConfiguration<ProcessoAdministrativo>
{
    public void Configure(EntityTypeBuilder<ProcessoAdministrativo> entity)
    {
        entity.ToTable("processo_administrativo");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.NumeroProcesso).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Orgao).HasMaxLength(100);
        entity.Property(e => e.Observacao).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);

        entity.HasIndex(e => e.EmpresaId);
    }
}
