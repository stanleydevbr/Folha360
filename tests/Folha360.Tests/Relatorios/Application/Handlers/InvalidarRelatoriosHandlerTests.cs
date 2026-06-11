using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Handlers;
using Folha360.Relatorios.Domain.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Folha360.Tests.Relatorios.Application.Handlers;

[Trait("Category", "Unit")]
public class InvalidarRelatoriosHandlerTests
{
    private readonly Mock<IAgendamentoRepository> _agendamentoRepositoryMock;
    private readonly Mock<ILogger<InvalidarRelatoriosHandler>> _loggerMock;
    private readonly InvalidarRelatoriosHandler _handler;

    public InvalidarRelatoriosHandlerTests()
    {
        _agendamentoRepositoryMock = new Mock<IAgendamentoRepository>();
        _loggerMock = new Mock<ILogger<InvalidarRelatoriosHandler>>();
        _handler = new InvalidarRelatoriosHandler(_agendamentoRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DeveInvalidarArquivosDoPeriodo()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var command = new InvalidarRelatoriosCommand { EmpresaId = empresaId, Periodo = periodo };

        _agendamentoRepositoryMock
            .Setup(r => r.InvalidarArquivosAsync(empresaId, periodo, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _agendamentoRepositoryMock.Verify(
            r => r.InvalidarArquivosAsync(empresaId, periodo, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRegistrarLog()
    {
        // Arrange
        var command = new InvalidarRelatoriosCommand { EmpresaId = Guid.NewGuid(), Periodo = "2026-05" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Invalidando relatórios")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
