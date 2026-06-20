using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade MovimentacaoMensal — rubricas específicas de um mês para o funcionário.
/// Schema: tenant.
/// </summary>
public class MovimentacaoMensalConfiguration : IEntityTypeConfiguration<MovimentacaoMensal>
{
    public void Configure(EntityTypeBuilder<MovimentacaoMensal> entity)
    {
        entity.ToTable("movimentacoes_mensais");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.MesAno).IsRequired().HasMaxLength(7);
        entity.Property(e => e.Descricao).HasMaxLength(200);
        entity.Property(e => e.Quantidade).HasColumnType("decimal(18,4)");
        entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId);
        entity.HasIndex(e => e.RubricaId);
        entity.HasIndex(e => new { e.FuncionarioId, e.RubricaId, e.MesAno }).IsUnique();
    }
}
