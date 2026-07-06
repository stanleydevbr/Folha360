using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade ConfiguracaoGeral — configurações chave-valor por empresa.
/// Schema: tenant. Não implementa ISoftDeletable (remoção direta).
/// </summary>
public class ConfiguracaoGeralConfiguration : IEntityTypeConfiguration<ConfiguracaoGeral>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoGeral> entity)
    {
        entity.ToTable("configuracoes_gerais");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Chave).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Valor).IsRequired().HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.EmpresaId);
        entity.HasIndex(e => new { e.EmpresaId, e.Chave }).IsUnique();
    }
}
