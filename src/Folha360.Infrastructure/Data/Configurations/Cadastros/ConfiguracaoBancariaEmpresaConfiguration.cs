using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade ConfiguracaoBancariaEmpresa — contas bancárias da empresa.
/// Schema: tenant.
/// </summary>
public class ConfiguracaoBancariaEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracaoBancariaEmpresa>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoBancariaEmpresa> entity)
    {
        entity.ToTable("configuracoes_bancarias_empresa");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Agencia).IsRequired().HasMaxLength(10);
        entity.Property(e => e.AgenciaDv).HasMaxLength(2);
        entity.Property(e => e.Conta).IsRequired().HasMaxLength(20);
        entity.Property(e => e.ContaDv).HasMaxLength(2);
        entity.Property(e => e.TipoConta).IsRequired().HasMaxLength(20);
        entity.Property(e => e.ChavePix).HasMaxLength(100);
        entity.Property(e => e.Finalidade).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Ativa).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => e.BancoId);
        entity.HasIndex(e => new { e.EmpresaId, e.Finalidade }).IsUnique();
    }
}
