using Folha360.Eventos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Eventos;

public class AdmissaoConfiguration : IEntityTypeConfiguration<Admissao>
{
    public void Configure(EntityTypeBuilder<Admissao> entity)
    {
        entity.ToTable("admissao");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FuncionarioId).IsRequired();
        entity.Property(e => e.EmpresaId).IsRequired();
        entity.Property(e => e.DataAdmissao).IsRequired();
        entity.Property(e => e.CargoId).IsRequired();
        entity.Property(e => e.SalarioInicial).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.TipoContrato).IsRequired().HasConversion<int>();
        entity.Property(e => e.PeriodoExperienciaMeses);
        entity.Property(e => e.XmlContent).HasColumnType("text");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId).HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.DataAdmissao);
    }
}
