using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folha360.Infrastructure.Data.Configurations.Cadastros;

/// <summary>
/// Configuração da entidade DadosBancariosFuncionario — contas bancárias do funcionário.
/// Schema: tenant.
/// </summary>
public class DadosBancariosFuncionarioConfiguration : IEntityTypeConfiguration<DadosBancariosFuncionario>
{
    public void Configure(EntityTypeBuilder<DadosBancariosFuncionario> entity)
    {
        entity.ToTable("dados_bancarios_funcionario");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Agencia).IsRequired().HasMaxLength(10);
        entity.Property(e => e.AgenciaDv).HasMaxLength(2);
        entity.Property(e => e.Conta).IsRequired().HasMaxLength(20);
        entity.Property(e => e.ContaDv).HasMaxLength(2);
        entity.Property(e => e.TipoConta).IsRequired().HasMaxLength(20);
        entity.Property(e => e.ChavePix).HasMaxLength(100);
        entity.Property(e => e.ContaPrincipal).IsRequired();
        entity.Property(e => e.CreatedAt).IsRequired();
        entity.Property(e => e.UpdatedAt).IsRequired();
        entity.Property(e => e.DeletedAt);
        entity.HasIndex(e => e.FuncionarioId);
        entity.HasIndex(e => e.BancoId);
    }
}
