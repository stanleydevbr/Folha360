using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class DirfAnualViewConfiguration : IEntityTypeConfiguration<DirfAnualView>
{
    public void Configure(EntityTypeBuilder<DirfAnualView> builder)
    {
        builder.HasNoKey().ToView("vw_dirf_anual");

        builder.Property(v => v.EmpresaId).HasColumnName("empresa_id");
        builder.Property(v => v.Ano).HasColumnName("ano");
        builder.Property(v => v.FuncionarioId).HasColumnName("funcionario_id");
        builder.Property(v => v.Cpf).HasColumnName("cpf");
        builder.Property(v => v.NomeFuncionario).HasColumnName("nome_funcionario");
        builder.Property(v => v.RendimentosTributaveis).HasColumnName("rendimentos_tributaveis").HasPrecision(18, 2);
        builder.Property(v => v.RendimentosIsentos).HasColumnName("rendimentos_isentos").HasPrecision(18, 2);
        builder.Property(v => v.IrrfRetido).HasColumnName("irrf_retido").HasPrecision(18, 2);
        builder.Property(v => v.DecimoTerceiro).HasColumnName("decimo_terceiro").HasPrecision(18, 2);
        builder.Property(v => v.Ferias).HasColumnName("ferias").HasPrecision(18, 2);
    }
}
