using Folha360.Eventos.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Eventos;

public class AfastamentoConfiguration : IEntityTypeConfiguration<Afastamento>
{
    public void Configure(EntityTypeBuilder<Afastamento> entity)
    {
        entity.ToTable("afastamento");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.DataInicio).IsRequired();
        entity.Property(e => e.DataFimPrevista).IsRequired();
        entity.Property(e => e.DataFimEfetiva);
        entity.Property(e => e.TipoAfastamento).IsRequired().HasConversion<int>();
        entity.Property(e => e.Cid).HasConversion<EncryptionConverter>();
        entity.Property(e => e.XmlContent).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
    }
}
