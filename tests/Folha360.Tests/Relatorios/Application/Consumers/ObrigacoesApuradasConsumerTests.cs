using Folha360.Application;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Events;
using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Consumers;
using Folha360.Relatorios.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;

namespace Folha360.Tests.Relatorios.Application.Consumers;

[Trait("Category", "Unit")]
public class ObrigacoesApuradasConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRedisCacheService> _cacheServiceMock;
    private readonly Mock<ILogger<ObrigacoesApuradasConsumer>> _loggerMock;
    private readonly ObrigacoesApuradasConsumer _consumer;

    public ObrigacoesApuradasConsumerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _cacheServiceMock = new Mock<IRedisCacheService>();
        _loggerMock = new Mock<ILogger<ObrigacoesApuradasConsumer>>();
        _consumer = new ObrigacoesApuradasConsumer(_mediatorMock.Object, _cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_DeveInvalidarCacheRedis()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var obrigacoes = new ObrigacoesApuradasEvent(
            empresaId, periodo, Guid.NewGuid(),
            new Dictionary<Tributo, decimal> { { Tributo.IRRF, 5000m }, { Tributo.INSS, 10000m } },
            DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<ObrigacoesApuradasEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(obrigacoes);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _cacheServiceMock.Verify(
            c => c.InvalidarAsync(
                $"relatorios:resumo:{empresaId}:{periodo}",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_DeveEnviarRefreshViewsCommand()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var obrigacoes = new ObrigacoesApuradasEvent(
            empresaId, periodo, Guid.NewGuid(),
            new Dictionary<Tributo, decimal> { { Tributo.FGTS, 8000m } },
            DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<ObrigacoesApuradasEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(obrigacoes);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<RefreshViewsCommand>(cmd => cmd.EmpresaId == empresaId && cmd.Periodo == periodo),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_DeveInvalidarCacheAntesDoRefresh()
    {
        // Arrange - verifica a ordem das operações
        var obrigacoes = new ObrigacoesApuradasEvent(
            Guid.NewGuid(), "2026-05", Guid.NewGuid(),
            new Dictionary<Tributo, decimal> { { Tributo.INSS, 1000m } },
            DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<ObrigacoesApuradasEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(obrigacoes);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        var sequencia = new List<string>();

        _cacheServiceMock
            .Setup(c => c.InvalidarAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback(() => sequencia.Add("cache"))
            .Returns(Task.CompletedTask);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .Callback(() => sequencia.Add("refresh"))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        Assert.Equal(new[] { "cache", "refresh" }, sequencia);
    }

    [Fact]
    public async Task Consume_DeveRegistrarLogInformation()
    {
        // Arrange
        var obrigacoes = new ObrigacoesApuradasEvent(
            Guid.NewGuid(), "2026-05", Guid.NewGuid(),
            new Dictionary<Tributo, decimal>(),
            DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<ObrigacoesApuradasEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(obrigacoes);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Consumindo ObrigacoesApuradas")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
