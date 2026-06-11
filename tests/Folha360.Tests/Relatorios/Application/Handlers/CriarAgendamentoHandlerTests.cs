using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Handlers;
using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;
using Moq;

namespace Folha360.Tests.Relatorios.Application.Handlers;

[Trait("Category", "Unit")]
public class CriarAgendamentoHandlerTests
{
    private readonly Mock<IAgendamentoService> _agendamentoServiceMock;
    private readonly CriarAgendamentoHandler _handler;

    public CriarAgendamentoHandlerTests()
    {
        _agendamentoServiceMock = new Mock<IAgendamentoService>();
        _handler = new CriarAgendamentoHandler(_agendamentoServiceMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarIdDoAgendamentoCriado()
    {
        var agendamentoId = Guid.NewGuid();
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = Folha360.Relatorios.Domain.Enums.TipoRelatorio.FolhaAnalitica,
            Formato = Folha360.Relatorios.Domain.Enums.FormatoExportacao.Pdf,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string> { "test@test.com" },
        };
        _agendamentoServiceMock.Setup(s => s.CriarAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(agendamentoId);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(agendamentoId, result.Value);
    }

    [Fact]
    public async Task Handle_DevePassarCommandParaService()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = Folha360.Relatorios.Domain.Enums.TipoRelatorio.ResumoMensal,
            Formato = Folha360.Relatorios.Domain.Enums.FormatoExportacao.Csv,
            Recorrencia = "0 0 10 * * ?",
            Destinatarios = new List<string> { "admin@test.com" },
        };
        _agendamentoServiceMock.Setup(s => s.CriarAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(Guid.NewGuid());
        await _handler.Handle(command, CancellationToken.None);
        _agendamentoServiceMock.Verify(s => s.CriarAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}
