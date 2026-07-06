using Folha360.Application;
using Folha360.Processamento.Domain.Events;
using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Consumers;
using Folha360.Relatorios.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;

namespace Folha360.Tests.Relatorios.Application.Consumers;

[Trait("Category", "Unit")]
public class FolhaFechadaConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<FolhaFechadaConsumer>> _loggerMock;
    private readonly FolhaFechadaConsumer _consumer;

    public FolhaFechadaConsumerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<FolhaFechadaConsumer>>();
        _consumer = new FolhaFechadaConsumer(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_DeveEnviarRefreshViewsCommand()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var processamentoId = Guid.NewGuid();
        var folhaFechada = new FolhaFechadaEvent(empresaId, periodo, processamentoId, 100000m, 30000m, 70000m, 50, DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaFechadaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaFechada);
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
    public async Task Consume_DevePublicarRelatoriosAtualizadosEvent()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var folhaFechada = new FolhaFechadaEvent(empresaId, "2026-05", Guid.NewGuid(), 100000m, 30000m, 70000m, 50, DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaFechadaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaFechada);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        consumeContextMock.Setup(c => c.CorrelationId).Returns(correlationId);
        consumeContextMock.Setup(c => c.MessageId).Returns(messageId);

        RelatoriosAtualizadosEvent? eventoPublicado = null;
        consumeContextMock
            .Setup(c => c.Publish(It.IsAny<RelatoriosAtualizadosEvent>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((obj, _) => eventoPublicado = obj as RelatoriosAtualizadosEvent)
            .Returns(Task.CompletedTask);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        Assert.NotNull(eventoPublicado);
        Assert.Equal(empresaId, eventoPublicado!.EmpresaId);
        Assert.Equal("2026-05", eventoPublicado.Periodo);
        Assert.Contains("folha_analitica", eventoPublicado.TiposRelatorio);
        Assert.Contains("folha_sintetica", eventoPublicado.TiposRelatorio);
        Assert.Contains("resumo_mensal", eventoPublicado.TiposRelatorio);
        Assert.Equal(correlationId, eventoPublicado.CorrelationId);
        Assert.Equal(messageId, eventoPublicado.CausationId);
    }

    [Fact]
    public async Task Consume_QuandoCorrelationIdNulo_DeveGerarNovoGuid()
    {
        // Arrange
        var folhaFechada = new FolhaFechadaEvent(Guid.NewGuid(), "2026-05", Guid.NewGuid(), 100000m, 30000m, 70000m, 50, DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaFechadaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaFechada);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        consumeContextMock.Setup(c => c.CorrelationId).Returns((Guid?)null);
        consumeContextMock.Setup(c => c.MessageId).Returns((Guid?)null);

        RelatoriosAtualizadosEvent? eventoPublicado = null;
        consumeContextMock
            .Setup(c => c.Publish(It.IsAny<RelatoriosAtualizadosEvent>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((obj, _) => eventoPublicado = obj as RelatoriosAtualizadosEvent)
            .Returns(Task.CompletedTask);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RefreshViewsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        Assert.NotNull(eventoPublicado);
        Assert.NotEqual(Guid.Empty, eventoPublicado!.CorrelationId);
        Assert.NotEqual(Guid.Empty, eventoPublicado.CausationId);
    }

    [Fact]
    public async Task Consume_DeveRegistrarLogInformation()
    {
        // Arrange
        var folhaFechada = new FolhaFechadaEvent(Guid.NewGuid(), "2026-05", Guid.NewGuid(), 100000m, 30000m, 70000m, 50, DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaFechadaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaFechada);
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
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Consumindo FolhaFechada")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
