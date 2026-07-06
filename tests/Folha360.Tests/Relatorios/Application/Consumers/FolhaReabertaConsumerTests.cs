using Folha360.Application;
using Folha360.Processamento.Domain.Events;
using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MediatR;

namespace Folha360.Tests.Relatorios.Application.Consumers;

[Trait("Category", "Unit")]
public class FolhaReabertaConsumerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<FolhaReabertaConsumer>> _loggerMock;
    private readonly FolhaReabertaConsumer _consumer;

    public FolhaReabertaConsumerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<FolhaReabertaConsumer>>();
        _consumer = new FolhaReabertaConsumer(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_DeveEnviarInvalidarRelatoriosCommand()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var folhaReaberta = new FolhaReabertaEvent(empresaId, periodo, Guid.NewGuid(), 1, "Erro no cálculo", "admin", DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaReabertaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaReaberta);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<InvalidarRelatoriosCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(
                It.Is<InvalidarRelatoriosCommand>(cmd => cmd.EmpresaId == empresaId && cmd.Periodo == periodo),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_DeveRegistrarLogInformation()
    {
        // Arrange
        var folhaReaberta = new FolhaReabertaEvent(Guid.NewGuid(), "2026-05", Guid.NewGuid(), 1, "Motivo", "autor", DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaReabertaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaReaberta);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<InvalidarRelatoriosCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Consumindo FolhaReaberta")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_DevePropagarPeriodoCorretamente()
    {
        // Arrange
        var periodoEsperado = "2026-12";
        var folhaReaberta = new FolhaReabertaEvent(Guid.NewGuid(), periodoEsperado, Guid.NewGuid(), 2, "Recálculo 13º", "sistema", DateTime.UtcNow);

        var consumeContextMock = new Mock<ConsumeContext<FolhaReabertaEvent>>();
        consumeContextMock.Setup(c => c.Message).Returns(folhaReaberta);
        consumeContextMock.Setup(c => c.CancellationToken).Returns(CancellationToken.None);

        InvalidarRelatoriosCommand? comandoEnviado = null;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<InvalidarRelatoriosCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest<Result<bool>>, CancellationToken>((req, _) => comandoEnviado = req as InvalidarRelatoriosCommand)
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        await _consumer.Consume(consumeContextMock.Object);

        // Assert
        Assert.NotNull(comandoEnviado);
        Assert.Equal(periodoEsperado, comandoEnviado!.Periodo);
    }
}
