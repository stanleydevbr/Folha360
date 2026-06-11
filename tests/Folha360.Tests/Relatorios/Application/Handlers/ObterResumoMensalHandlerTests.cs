using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Handlers;
using Folha360.Relatorios.Application.Queries;
using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Moq;

namespace Folha360.Tests.Relatorios.Application.Handlers;

[Trait("Category", "Unit")]
public class ObterResumoMensalHandlerTests
{
    private readonly Mock<IRelatorioRepository> _repositoryMock;
    private readonly ObterResumoMensalHandler _handler;

    public ObterResumoMensalHandlerTests()
    {
        _repositoryMock = new Mock<IRelatorioRepository>();
        _handler = new ObterResumoMensalHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarResumoMensalCompleto()
    {
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var view = new ResumoMensalView
        {
            TotalFuncionarios = 150,
            TotalVencimentos = 500000m,
            TotalDescontos = 150000m,
            TotalLiquido = 350000m,
            TotalIrrf = 30000m,
            TotalInss = 60000m,
            TotalFgts = 40000m,
        };
        _repositoryMock.Setup(r => r.ObterResumoMensalAsync(empresaId, periodo, It.IsAny<CancellationToken>())).ReturnsAsync(view);
        var query = new ObterResumoMensalQuery { EmpresaId = empresaId, Periodo = periodo };
        var result = await _handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        var value = result.Value;
        Assert.NotNull(value);
        Assert.Equal(150, value.TotalFuncionarios);
        Assert.Equal(500000m, value.TotalVencimentos);
        Assert.Equal(150000m, value.TotalDescontos);
        Assert.Equal(350000m, value.TotalLiquido);
        Assert.Equal(30000m, value.TotalIrrf);
        Assert.Equal(60000m, value.TotalInss);
        Assert.Equal(40000m, value.TotalFgts);
    }

    [Fact]
    public async Task Handle_DeveMapearPeriodoCorretamente()
    {
        var empresaId = Guid.NewGuid();
        var periodo = "2026-12";
        _repositoryMock.Setup(r => r.ObterResumoMensalAsync(empresaId, periodo, It.IsAny<CancellationToken>())).ReturnsAsync(new ResumoMensalView());
        var query = new ObterResumoMensalQuery { EmpresaId = empresaId, Periodo = periodo };
        var result = await _handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsSuccess);
        var value = result.Value;
        Assert.NotNull(value);
        Assert.Equal(empresaId, query.EmpresaId);
        Assert.Equal(periodo, query.Periodo);
    }
}
