using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Handlers;
using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Domain.Abstractions;
using MassTransit;
using Moq;

namespace Folha360.Tests.Eventos.Handlers;

[Trait("Category", "Unit")]
public class CriarAdmissaoHandlerTests
{
    private readonly Mock<IAdmissaoRepository> _repoMock = new();
    private readonly Mock<IMessageBus> _messageBusMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();

    [Fact]
    public async Task DeveCriarAdmissao_EPublicarEventos()
    {
        // Arrange
        var handler = new CriarAdmissaoHandler(_repoMock.Object, _messageBusMock.Object, _publishEndpointMock.Object);
        var cmd = new CriarAdmissaoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataAdmissao = new DateOnly(2026, 6, 1),
            CargoId = Guid.NewGuid(),
            SalarioInicial = 5000m,
            TipoContrato = TipoContrato.Indeterminado,
        };

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(cmd.FuncionarioId, result.Value.FuncionarioId);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Admissao>(), It.IsAny<CancellationToken>()), Times.Once);
        _messageBusMock.Verify(m => m.PublishAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _publishEndpointMock.Verify(p => p.Publish(
            It.IsAny<Folha360.Eventos.Domain.Events.GerarXmlAdmissaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ObterAdmissaoHandlerTests
{
    [Fact]
    public async Task DeveRetornarAdmissao_QuandoEncontrada()
    {
        // Arrange
        var repoMock = new Mock<IAdmissaoRepository>();
        var entity = new Admissao(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), Guid.NewGuid(), 5000m, TipoContrato.Indeterminado);
        repoMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new ObterAdmissaoHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new Folha360.Eventos.Application.Queries.ObterAdmissaoQuery(entity.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(entity.Id, result.Value!.Id);
    }

    [Fact]
    public async Task DeveRetornarFalha_QuandoNaoEncontrada()
    {
        // Arrange
        var repoMock = new Mock<IAdmissaoRepository>();
        repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Admissao?)null);

        var handler = new ObterAdmissaoHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new Folha360.Eventos.Application.Queries.ObterAdmissaoQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
