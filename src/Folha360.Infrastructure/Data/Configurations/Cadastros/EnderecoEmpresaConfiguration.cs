using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade EnderecoEmpresa — múltiplos endereços por tipo.
/// Schema: tenant.
/// </summary>
public class EnderecoEmpresaConfiguration : IEntityTypeConfiguration<EnderecoEmpresa>
{
    public void Configure(EntityTypeBuilder<EnderecoEmpresa> entity)
    {
        entity.ToTable("enderecos_empresa");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Logradouro).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Numero).HasMaxLength(20);
        entity.Property(e => e.Complemento).HasMaxLength(100);
        entity.Property(e => e.Bairro).HasMaxLength(100);
        entity.Property(e => e.Cep).HasMaxLength(10);
        entity.Property(e => e.Uf).HasMaxLength(2);
        entity.Property(e => e.Estrangeiro).IsRequired();
        entity.Property(e => e.Pais).HasMaxLength(50);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.MunicipioId);
        entity.HasIndex(e => new { e.EmpresaId, e.Tipo }).IsUnique();
    }
}
