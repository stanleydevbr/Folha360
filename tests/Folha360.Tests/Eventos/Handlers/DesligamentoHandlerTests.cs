using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Handlers;
using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Domain.Abstractions;
using MassTransit;
using Moq;

namespace Folha360.Tests.Eventos.Handlers;

public class CriarDesligamentoHandlerTests
{
    [Fact]
    public async Task DeveCriarDesligamento_EPublicarEventos()
    {
        // Arrange
        var repoMock = new Mock<IDesligamentoRepository>();
        var messageBusMock = new Mock<IMessageBus>();
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        var handler = new CriarDesligamentoHandler(repoMock.Object, messageBusMock.Object, publishEndpointMock.Object);
        var cmd = new CriarDesligamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataDesligamento = new DateOnly(2026, 6, 30),
            MotivoDesligamento = MotivoDesligamento.SemJustaCausa,
        };

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        repoMock.Verify(r => r.AddAsync(It.IsAny<Desligamento>(), It.IsAny<CancellationToken>()), Times.Once);
        messageBusMock.Verify(m => m.PublishAsync(
            It.IsAny<object>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        publishEndpointMock.Verify(p => p.Publish(
            It.IsAny<Folha360.Eventos.Domain.Events.GerarXmlDesligamentoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
