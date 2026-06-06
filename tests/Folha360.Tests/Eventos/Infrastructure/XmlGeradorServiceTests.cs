using Folha360.Cadastros.Domain.Entities;
using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Folha360.Tests.Eventos.Infrastructure;

public class XmlGeradorServiceTests
{
    private readonly XmlGeradorService _service;
    private readonly Empresa _empresaMock;
    private readonly Funcionario _funcionarioMock;

    public XmlGeradorServiceTests()
    {
        var loggerMock = new Mock<ILogger<XmlGeradorService>>();
        _service = new XmlGeradorService(loggerMock.Object);

        _empresaMock = new Empresa(
            Guid.NewGuid(),
            "12345678000199",
            "Empresa Teste LTDA",
            "Lucro Presumido",
            "Empresa Teste");

        _funcionarioMock = new Funcionario(
            Guid.NewGuid(),
            "Funcionario Teste",
            "12345678901",
            "hash_cpf",
            new DateOnly(2025, 1, 1),
            Guid.NewGuid(),
            Guid.NewGuid(),
            3000m,
            new DateOnly(1990, 1, 1),
            "Masculino");
    }

    [Fact]
    public void GerarXmlAdmissao_DeveGerarXmlValido()
    {
        // Arrange
        var admissao = new Admissao(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            5000m,
            TipoContrato.Indeterminado);

        // Act
        var xml = _service.GerarXmlAdmissao(admissao, _empresaMock, _funcionarioMock);

        // Assert
        Assert.NotNull(xml);
        Assert.NotEmpty(xml);
        Assert.Contains("<eSocial", xml);
        Assert.Contains("<evtAdmissao", xml);
        Assert.Contains("<dtAdm>2026-06-01</dtAdm>", xml);
        Assert.Contains("<salario>5000.00</salario>", xml);
        Assert.Contains("12345678000199", xml);
        Assert.Contains("Funcionario Teste", xml);
    }

    [Fact]
    public void GerarXmlFerias_DeveGerarXmlValido()
    {
        // Arrange
        var ferias = new Ferias(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 12, 1),
            30,
            new DateOnly(2025, 6, 1),
            new DateOnly(2026, 5, 31),
            TipoFerias.Normais);

        // Act
        var xml = _service.GerarXmlFerias(ferias, _empresaMock, _funcionarioMock);

        // Assert
        Assert.NotNull(xml);
        Assert.Contains("<evtFerias", xml);
        Assert.Contains("<dtInicio>2026-12-01</dtInicio>", xml);
        Assert.Contains("<diasGozo>30</diasGozo>", xml);
    }

    [Fact]
    public void GerarXmlAfastamento_DeveGerarXmlValido()
    {
        // Arrange
        var afastamento = new Afastamento(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 15),
            TipoAfastamento.Doenca,
            "J11.0");

        // Act
        var xml = _service.GerarXmlAfastamento(afastamento, _empresaMock, _funcionarioMock);

        // Assert
        Assert.NotNull(xml);
        Assert.Contains("<evtAfastTemp", xml);
        Assert.Contains("<dtInicio>2026-06-01</dtInicio>", xml);
        Assert.Contains("<dtFimPrev>2026-06-15</dtFimPrev>", xml);
    }

    [Fact]
    public void GerarXmlDesligamento_DeveGerarXmlValido()
    {
        // Arrange
        var desligamento = new Desligamento(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 6, 30),
            MotivoDesligamento.SemJustaCausa);

        // Act
        var xml = _service.GerarXmlDesligamento(desligamento, _empresaMock, _funcionarioMock);

        // Assert
        Assert.NotNull(xml);
        Assert.Contains("<evtDeslig", xml);
        Assert.Contains("<dtDeslig>2026-06-30</dtDeslig>", xml);
    }

    [Fact]
    public void GerarXmlAlteracaoContratual_DeveGerarXmlValido()
    {
        // Arrange
        var alteracao = new AlteracaoContratual(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 7, 1),
            "{\"salario\":true}",
            "{\"salario\":5000}",
            "{\"salario\":6000}");

        // Act
        var xml = _service.GerarXmlAlteracaoContratual(alteracao, _empresaMock, _funcionarioMock);

        // Assert
        Assert.NotNull(xml);
        Assert.Contains("<evtAltContratual", xml);
        Assert.Contains("<dtAlteracao>2026-07-01</dtAlteracao>", xml);
    }

    [Fact]
    public void ValidarContraXsd_DeveRetornarValido_ParaXmlValido()
    {
        // Arrange
        var admissao = new Admissao(
            _funcionarioMock.Id,
            _empresaMock.Id,
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            5000m,
            TipoContrato.Indeterminado);
        var xml = _service.GerarXmlAdmissao(admissao, _empresaMock, _funcionarioMock);

        // Act
        var result = _service.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.S-2200.xsd");

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidarContraXsd_DeveRetornarInvalido_ParaXmlInvalido()
    {
        // Arrange
        var xmlInvalido = "<eSocial><evtInvalido></evtInvalido></eSocial>";

        // Act
        var result = _service.ValidarContraXsd(xmlInvalido, "Folha360.Eventos.Infrastructure.Xsd.S-2200.xsd");

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void ValidarContraXsd_DeveRetornarComWarning_ParaXsdInexistente()
    {
        // Arrange
        var xml = "<eSocial></eSocial>";

        // Act
        var result = _service.ValidarContraXsd(xml, "Folha360.Eventos.Infrastructure.Xsd.Inexistente.xsd");

        // Assert
        Assert.True(result.IsValid);
        Assert.NotEmpty(result.Warnings);
    }
}
