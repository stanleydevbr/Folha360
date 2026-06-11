using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Handlers;
using Folha360.Relatorios.Application.Queries;
using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Moq;

namespace Folha360.Tests.Relatorios.Application.Handlers;

[Trait("Category", "Unit")]
public class ObterDirfHandlerTests
{
    private readonly Mock<IRelatorioRepository> _repositoryMock;
    private readonly ObterDirfHandler _handler;

    public ObterDirfHandlerTests()
    {
        _repositoryMock = new Mock<IRelatorioRepository>();
        _handler = new ObterDirfHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaDeDirfDto()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var ano = 2026;
        var views = new List<DirfAnualView>
        {
            new() { FuncionarioId = Guid.NewGuid(), Cpf = "12345678901", NomeFuncionario = "João", RendimentosTributaveis = 50000m, RendimentosIsentos = 10000m, IrrfRetido = 7500m, DecimoTerceiro = 5000m, Ferias = 4000m },
            new() { FuncionarioId = Guid.NewGuid(), Cpf = "98765432100", NomeFuncionario = "Maria", RendimentosTributaveis = 60000m, RendimentosIsentos = 5000m, IrrfRetido = 9000m, DecimoTerceiro = 6000m, Ferias = 5000m },
        };

        _repositoryMock
            .Setup(r => r.ObterDirfAnualAsync(empresaId, ano, It.IsAny<CancellationToken>()))
            .ReturnsAsync(views);

        var query = new ObterDirfQuery { EmpresaId = empresaId, Ano = ano };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("João", result.Value[0].Nome);
        Assert.Equal(50000m, result.Value[0].RendimentosTributaveis);
        Assert.Equal(7500m, result.Value[0].IrrfRetido);
        Assert.Equal("Maria", result.Value[1].Nome);
    }

    [Fact]
    public async Task Handle_QuandoNaoHaDados_DeveRetornarListaVazia()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ObterDirfAnualAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DirfAnualView>());

        var query = new ObterDirfQuery { EmpresaId = Guid.NewGuid(), Ano = 2026 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_DevePassarParametrosCorretosParaRepository()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var ano = 2026;

        _repositoryMock
            .Setup(r => r.ObterDirfAnualAsync(empresaId, ano, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DirfAnualView>());

        var query = new ObterDirfQuery { EmpresaId = empresaId, Ano = ano };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.ObterDirfAnualAsync(empresaId, ano, It.IsAny<CancellationToken>()), Times.Once);
    }
}
