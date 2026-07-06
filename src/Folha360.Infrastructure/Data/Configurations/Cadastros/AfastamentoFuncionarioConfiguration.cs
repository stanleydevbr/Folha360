using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade AfastamentoFuncionario — afastamentos do funcionário.
/// Schema: tenant.
/// </summary>
public class AfastamentoFuncionarioConfiguration : IEntityTypeConfiguration<AfastamentoFuncionario>
{
    public void Configure(EntityTypeBuilder<AfastamentoFuncionario> entity)
    {
        entity.ToTable("afastamentos_funcionario");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
        entity.Property(e => e.DataInicio).IsRequired();
        entity.Property(e => e.NumeroAtestadoCid).HasMaxLength(50);
        entity.Property(e => e.Observacoes).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId);
        entity.HasIndex(e => e.Tipo);
        entity.HasIndex(e => e.DataInicio);
    }
}
