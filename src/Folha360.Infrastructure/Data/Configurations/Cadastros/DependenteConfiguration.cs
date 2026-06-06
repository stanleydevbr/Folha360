using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class DependenteConfiguration : IEntityTypeConfiguration<Dependente>
{
    public void Configure(EntityTypeBuilder<Dependente> entity)
    {
        entity.ToTable("dependente");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cpf).IsRequired().HasConversion<EncryptionConverter>();
        entity.Property(e => e.DataNascimento).IsRequired();
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.GrauParentesco).HasMaxLength(50);
        entity.Property(e => e.PensaoAlimenticiaValor).HasColumnType("decimal(18,2)");
        entity.Property(e => e.PensaoAlimenticiaPercentual).HasColumnType("decimal(5,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId);
    }
}
