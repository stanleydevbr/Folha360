using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Tests.Cadastros.Domain;

[Trait("Category", "Unit")]
public class ExpandedEntitiesTests
{
    [Fact]
    public void ContratoTrabalho_PrazoDeterminado_SemDataTermino_DeveLancarExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(() => new ContratoTrabalho(
            Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1),
            "CLT_DETERMINADO", 2000m, "MENSALISTA"));

        Assert.Contains("Data de término", ex.Message);
    }

    [Fact]
    public void ContratoTrabalho_PrazoDeterminado_ComDataTermino_DeveCriar()
    {
        var contrato = new ContratoTrabalho(
            Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1),
            "CLT_DETERMINADO", 2000m, "MENSALISTA",
            dataTerminoContrato: new DateOnly(2026, 12, 31));

        Assert.Equal("CLT_DETERMINADO", contrato.TipoContrato);
        Assert.Equal(new DateOnly(2026, 12, 31), contrato.DataTerminoContrato);
    }

    [Fact]
    public void ContratoTrabalho_Desligar_DeveAtualizarStatusEData()
    {
        var contrato = new ContratoTrabalho(
            Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1),
            "CLT_INDETERMINADO", 2000m, "MENSALISTA");

        var dataDesligamento = new DateOnly(2026, 6, 15);
        contrato.Desligar(dataDesligamento);

        Assert.Equal("DESLIGADO", contrato.Status);
        Assert.Equal(dataDesligamento, contrato.DataDesligamento);
    }

    [Fact]
    public void EnderecoEmpresa_Estrangeiro_SemPais_DeveLancarExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EnderecoEmpresa(
            Guid.NewGuid(), "PRINCIPAL", "Main Street",
            estrangeiro: true));

        Assert.Contains("País", ex.Message);
    }

    [Fact]
    public void EnderecoEmpresa_Estrangeiro_ComPais_DeveCriar()
    {
        var endereco = new EnderecoEmpresa(
            Guid.NewGuid(), "PRINCIPAL", "Main Street",
            estrangeiro: true, pais: "United States");

        Assert.True(endereco.Estrangeiro);
        Assert.Equal("United States", endereco.Pais);
    }

    [Fact]
    public void EnderecoEmpresa_Nacional_DeveCriarSemPais()
    {
        var endereco = new EnderecoEmpresa(
            Guid.NewGuid(), "FISCAL", "Av Paulista", "1000",
            municipioId: Guid.NewGuid(), uf: "SP");

        Assert.False(endereco.Estrangeiro);
        Assert.Equal("FISCAL", endereco.Tipo);
    }

    [Fact]
    public void InfoESocialFuncionario_DeficienciaSemTipo_DeveLancarExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InfoESocialFuncionario(
            Guid.NewGuid(), indicadorDeficiencia: true));

        Assert.Contains("deficiência", ex.Message.ToLower());
    }

    [Fact]
    public void InfoESocialFuncionario_DeficienciaComTipo_DeveCriar()
    {
        var info = new InfoESocialFuncionario(
            Guid.NewGuid(), indicadorDeficiencia: true, tipoDeficiencia: "FISICA");

        Assert.True(info.IndicadorDeficiencia);
        Assert.Equal("FISICA", info.TipoDeficiencia);
    }

    [Fact]
    public void InfoESocialFuncionario_SemDeficiencia_DeveCriar()
    {
        var info = new InfoESocialFuncionario(
            Guid.NewGuid(), primeiroEmprego: true, reservista: true);

        Assert.False(info.IndicadorDeficiencia);
        Assert.True(info.PrimeiroEmprego);
        Assert.True(info.Reservista);
    }

    [Fact]
    public void ConfiguracaoESocial_EstaProximoVencimento_Dentro30Dias_DeveRetornarTrue()
    {
        var config = new ConfiguracaoESocial(
            Guid.NewGuid(), "PRODUCAO",
            certificadoVencimento: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)));

        Assert.True(config.EstaProximoVencimento());
    }

    [Fact]
    public void ConfiguracaoESocial_EstaProximoVencimento_Fora30Dias_DeveRetornarFalse()
    {
        var config = new ConfiguracaoESocial(
            Guid.NewGuid(), "PRODUCAO",
            certificadoVencimento: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60)));

        Assert.False(config.EstaProximoVencimento());
    }

    [Fact]
    public void ConfiguracaoESocial_EstaProximoVencimento_SemCertificado_DeveRetornarFalse()
    {
        var config = new ConfiguracaoESocial(Guid.NewGuid(), "PRODUCAO_RESTRITA");

        Assert.False(config.EstaProximoVencimento());
    }

    [Fact]
    public void AfastamentoFuncionario_DataFimAnteriorAInicio_DeveLancarExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(() => new AfastamentoFuncionario(
            Guid.NewGuid(), "DOENCA", new DateOnly(2026, 6, 15),
            dataFimPrevista: new DateOnly(2026, 6, 10)));

        Assert.Contains("não pode ser anterior", ex.Message);
    }

    [Fact]
    public void AfastamentoFuncionario_DatasValidas_DeveCriar()
    {
        var afastamento = new AfastamentoFuncionario(
            Guid.NewGuid(), "MATERNIDADE", new DateOnly(2026, 6, 1),
            dataFimPrevista: new DateOnly(2026, 11, 1),
            numeroAtestadoCid: "O80");

        Assert.Equal("MATERNIDADE", afastamento.Tipo);
        Assert.Equal("O80", afastamento.NumeroAtestadoCid);
    }

    [Fact]
    public void AfastamentoFuncionario_RegistrarRetorno_DeveAtualizarDataFimEfetiva()
    {
        var afastamento = new AfastamentoFuncionario(
            Guid.NewGuid(), "DOENCA", new DateOnly(2026, 6, 1));

        var dataRetorno = new DateOnly(2026, 6, 15);
        afastamento.RegistrarRetorno(dataRetorno);

        Assert.Equal(dataRetorno, afastamento.DataFimEfetiva);
    }

    [Fact]
    public void MovimentacaoMensal_MesAnoNulo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentNullException>(() => new MovimentacaoMensal(
            Guid.NewGuid(), Guid.NewGuid(), null!, 100m));
    }

    [Fact]
    public void MovimentacaoMensal_DadosValidos_DeveCriar()
    {
        var mov = new MovimentacaoMensal(
            Guid.NewGuid(), Guid.NewGuid(), "2026-06", 150m,
            descricao: "Horas Extras", quantidade: 10m);

        Assert.Equal("2026-06", mov.MesAno);
        Assert.Equal(150m, mov.Valor);
        Assert.Equal(10m, mov.Quantidade);
    }
}
