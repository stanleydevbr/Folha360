using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> entity)
    {
        entity.ToTable("funcionario");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cpf).IsRequired().HasConversion<EncryptionConverter>();
        entity.Property(e => e.CpfHash).IsRequired().HasMaxLength(64);
        entity.Property(e => e.DataAdmissao).IsRequired();
        entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Ativo");
        entity.Property(e => e.SalarioBase).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.Sexo).HasMaxLength(20);
        entity.Property(e => e.EstadoCivil).HasMaxLength(20);
        entity.Property(e => e.Nacionalidade).HasMaxLength(50);
        entity.Property(e => e.NomeMae).HasMaxLength(200);
        entity.Property(e => e.NomePai).HasMaxLength(200);
        entity.Property(e => e.EnderecoLogradouro).HasMaxLength(200);
        entity.Property(e => e.EnderecoNumero).HasMaxLength(20);
        entity.Property(e => e.EnderecoComplemento).HasMaxLength(100);
        entity.Property(e => e.EnderecoBairro).HasMaxLength(100);
        entity.Property(e => e.EnderecoCep).HasMaxLength(8);
        entity.Property(e => e.EnderecoMunicipio).HasMaxLength(100);
        entity.Property(e => e.EnderecoUf).HasMaxLength(2);
        entity.Property(e => e.Telefone).HasMaxLength(20);
        entity.Property(e => e.Email).HasMaxLength(255);
        entity.Property(e => e.TipoContrato).HasMaxLength(30);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.CpfHash).IsUnique().HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.CargoId);
        entity.HasIndex(e => e.LotacaoId);
        entity.HasIndex(e => e.Status);
        entity.HasIndex(e => e.Nome).HasMethod("gin").HasOperators("gin_trgm_ops");
    }
}
