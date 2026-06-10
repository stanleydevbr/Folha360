using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class ItemFolhaViewConfiguration : IEntityTypeConfiguration<ItemFolhaView>
{
    public void Configure(EntityTypeBuilder<ItemFolhaView> builder)
    {
        builder.HasNoKey().ToView("vw_resumo_folha_mensal");

        builder.Property(v => v.EmpresaId).HasColumnName("empresa_id");
        builder.Property(v => v.Periodo).HasColumnName("periodo");
        builder.Property(v => v.FuncionarioId).HasColumnName("funcionario_id");
        builder.Property(v => v.NomeFuncionario).HasColumnName("nome_funcionario");
        builder.Property(v => v.DepartamentoId).HasColumnName("departamento_id");
        builder.Property(v => v.NomeDepartamento).HasColumnName("nome_departamento");
        builder.Property(v => v.RubricaId).HasColumnName("rubrica_id");
        builder.Property(v => v.CodigoRubrica).HasColumnName("codigo_rubrica");
        builder.Property(v => v.NomeRubrica).HasColumnName("nome_rubrica");
        builder.Property(v => v.Natureza).HasColumnName("natureza");
        builder.Property(v => v.TipoCalculo).HasColumnName("tipo_calculo");
        builder.Property(v => v.Valor).HasColumnName("valor").HasPrecision(18, 2);
    }
}
