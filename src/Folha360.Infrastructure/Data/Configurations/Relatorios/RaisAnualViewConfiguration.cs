using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Infrastructure.Data.Configurations.Relatorios;

public class RaisAnualViewConfiguration : IEntityTypeConfiguration<RaisAnualView>
{
    public void Configure(EntityTypeBuilder<RaisAnualView> builder)
    {
        builder.HasNoKey().ToView("vw_rais_anual");

        builder.Property(v => v.EmpresaId).HasColumnName("empresa_id");
        builder.Property(v => v.Ano).HasColumnName("ano");
        builder.Property(v => v.FuncionarioId).HasColumnName("funcionario_id");
        builder.Property(v => v.Cpf).HasColumnName("cpf");
        builder.Property(v => v.NomeFuncionario).HasColumnName("nome_funcionario");
        builder.Property(v => v.PisPasep).HasColumnName("pis_pasep");
        builder.Property(v => v.DataAdmissao).HasColumnName("data_admissao");
        builder.Property(v => v.DataDesligamento).HasColumnName("data_desligamento");
        builder.Property(v => v.MotivoDesligamento).HasColumnName("motivo_desligamento");
        builder.Property(v => v.RemuneracaoJaneiro).HasColumnName("remuneracao_janeiro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoFevereiro).HasColumnName("remuneracao_fevereiro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoMarco).HasColumnName("remuneracao_marco").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoAbril).HasColumnName("remuneracao_abril").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoMaio).HasColumnName("remuneracao_maio").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoJunho).HasColumnName("remuneracao_junho").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoJulho).HasColumnName("remuneracao_julho").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoAgosto).HasColumnName("remuneracao_agosto").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoSetembro).HasColumnName("remuneracao_setembro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoOutubro).HasColumnName("remuneracao_outubro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoNovembro).HasColumnName("remuneracao_novembro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoDezembro).HasColumnName("remuneracao_dezembro").HasPrecision(18, 2);
        builder.Property(v => v.RemuneracaoTotal).HasColumnName("remuneracao_total").HasPrecision(18, 2);
        builder.Property(v => v.DecimoTerceiro).HasColumnName("decimo_terceiro").HasPrecision(18, 2);
    }
}
