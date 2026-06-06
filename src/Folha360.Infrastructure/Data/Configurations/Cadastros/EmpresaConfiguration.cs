using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> entity)
    {
        entity.ToTable("empresa", "public");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
        entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
        entity.Property(e => e.NomeFantasia).HasMaxLength(200);
        entity.Property(e => e.Cnae).HasMaxLength(7);
        entity.Property(e => e.RegimeTributario).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Fpas).HasMaxLength(10);
        entity.Property(e => e.CodigoTerceiros).HasMaxLength(50);
        entity.Property(e => e.ClassificacaoTributaria).HasMaxLength(30);
        entity.Property(e => e.MatrizFilial).HasMaxLength(10);
        entity.Property(e => e.CnpjMatriz).HasMaxLength(14);
        entity.Property(e => e.EnderecoLogradouro).HasMaxLength(200);
        entity.Property(e => e.EnderecoNumero).HasMaxLength(20);
        entity.Property(e => e.EnderecoComplemento).HasMaxLength(100);
        entity.Property(e => e.EnderecoBairro).HasMaxLength(100);
        entity.Property(e => e.EnderecoCep).HasMaxLength(8);
        entity.Property(e => e.EnderecoMunicipio).HasMaxLength(100);
        entity.Property(e => e.EnderecoUf).HasMaxLength(2);
        entity.Property(e => e.Telefone).HasMaxLength(20);
        entity.Property(e => e.Email).HasMaxLength(255);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.Cnpj).IsUnique().HasFilter("deleted_at IS NULL");
        entity.HasIndex(e => e.TenantId);
        entity.HasIndex(e => e.RazaoSocial);
    }
}
