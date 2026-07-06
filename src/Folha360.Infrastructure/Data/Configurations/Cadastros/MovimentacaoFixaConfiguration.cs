using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade MovimentacaoFixa — rubricas fixas mensais do funcionário.
/// Schema: tenant.
/// </summary>
public class MovimentacaoFixaConfiguration : IEntityTypeConfiguration<MovimentacaoFixa>
{
    public void Configure(EntityTypeBuilder<MovimentacaoFixa> entity)
    {
        entity.ToTable("movimentacoes_fixas");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Descricao).HasMaxLength(200);
        entity.Property(e => e.Quantidade).HasColumnType("decimal(18,4)");
        entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId);
        entity.HasIndex(e => e.RubricaId);
        entity.HasIndex(e => new { e.FuncionarioId, e.RubricaId }).IsUnique();
    }
}
