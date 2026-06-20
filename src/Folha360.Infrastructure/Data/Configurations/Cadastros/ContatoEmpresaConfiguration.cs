using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade ContatoEmpresa — contatos da empresa.
/// Schema: tenant.
/// </summary>
public class ContatoEmpresaConfiguration : IEntityTypeConfiguration<ContatoEmpresa>
{
    public void Configure(EntityTypeBuilder<ContatoEmpresa> entity)
    {
        entity.ToTable("contatos_empresa");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Cpf).HasMaxLength(14);
        entity.Property(e => e.Cargo).HasMaxLength(100);
        entity.Property(e => e.Email).HasMaxLength(200);
        entity.Property(e => e.Telefone).HasMaxLength(20);
        entity.Property(e => e.Celular).HasMaxLength(20);
        entity.Property(e => e.ContatoPrincipal).IsRequired();
        entity.Property(e => e.Ativo).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
    }
}
