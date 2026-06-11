using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Application.Handlers;
using Folha360.Relatorios.Application.Queries;
using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Moq;

namespace Folha360.Tests.Relatorios.Application.Handlers;

[Trait("Category", "Unit")]
public class ObterFolhaAnaliticaHandlerTests
{
    private readonly Mock<IRelatorioRepository> _repositoryMock;
    private readonly ObterFolhaAnaliticaHandler _handler;

    public ObterFolhaAnaliticaHandlerTests()
    {
        _repositoryMock = new Mock<IRelatorioRepository>();
        _handler = new ObterFolhaAnaliticaHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveAgruparItensPorFuncionario()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var funcionarioId = Guid.NewGuid();
        var itens = new List<ItemFolhaView>
        {
            new() { FuncionarioId = funcionarioId, NomeFuncionario = "João", NomeDepartamento = "TI", CodigoRubrica = "SALARIO", NomeRubrica = "Salário Base", Natureza = "VENCIMENTO", Valor = 5000m },
            new() { FuncionarioId = funcionarioId, NomeFuncionario = "João", NomeDepartamento = "TI", CodigoRubrica = "INSS", NomeRubrica = "INSS", Natureza = "DESCONTO", Valor = -550m },
            new() { FuncionarioId = funcionarioId, NomeFuncionario = "João", NomeDepartamento = "TI", CodigoRubrica = "IRRF", NomeRubrica = "IRRF", Natureza = "DESCONTO", Valor = -300m },
        };

        _repositoryMock
            .Setup(r => r.ObterFolhaAnaliticaAsync(empresaId, "2026-05", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itens);

        var query = new ObterFolhaAnaliticaQuery { EmpresaId = empresaId, Periodo = "2026-05" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Funcionarios);
        var func = result.Value.Funcionarios[0];
        Assert.Equal("João", func.Nome);
        Assert.Equal("TI", func.Departamento);
        Assert.Single(func.Vencimentos);
        Assert.Equal(2, func.Descontos.Count);
        Assert.Equal(5000m, func.TotalVencimentos);
        Assert.Equal(850m, func.TotalDescontos);
        Assert.Equal(5850m, func.Liquido);
    }

    [Fact]
    public async Task Handle_DeveCalcularLiquidoCorretamente()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var funcId = Guid.NewGuid();
        var itens = new List<ItemFolhaView>
        {
            new() { FuncionarioId = funcId, NomeFuncionario = "Maria", NomeDepartamento = "RH", CodigoRubrica = "SALARIO", NomeRubrica = "Salário", Natureza = "VENCIMENTO", Valor = 10000m },
            new() { FuncionarioId = funcId, NomeFuncionario = "Maria", NomeDepartamento = "RH", CodigoRubrica = "BONUS", NomeRubrica = "Bônus", Natureza = "VENCIMENTO", Valor = 2000m },
            new() { FuncionarioId = funcId, NomeFuncionario = "Maria", NomeDepartamento = "RH", CodigoRubrica = "INSS", NomeRubrica = "INSS", Natureza = "DESCONTO", Valor = -908.85m },
            new() { FuncionarioId = funcId, NomeFuncionario = "Maria", NomeDepartamento = "RH", CodigoRubrica = "IRRF", NomeRubrica = "IRRF", Natureza = "DESCONTO", Valor = -1800m },
        };

        _repositoryMock
            .Setup(r => r.ObterFolhaAnaliticaAsync(empresaId, "2026-05", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itens);

        var query = new ObterFolhaAnaliticaQuery { EmpresaId = empresaId, Periodo = "2026-05" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var func = result.Value.Funcionarios[0];
        Assert.Equal(12000m, func.TotalVencimentos);
        Assert.Equal(2708.85m, func.TotalDescontos);
        Assert.Equal(14708.85m, func.Liquido);
    }

    [Fact]
    public async Task Handle_DeveAplicarPaginacao()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var itens = new List<ItemFolhaView>();
        for (int i = 0; i < 25; i++)
        {
            var funcId = Guid.NewGuid();
            itens.Add(new ItemFolhaView
            {
                FuncionarioId = funcId,
                NomeFuncionario = $"Funcionario {i}",
                NomeDepartamento = "TI",
                CodigoRubrica = "SALARIO",
                NomeRubrica = "Salário",
                Natureza = "VENCIMENTO",
                Valor = 5000m,
            });
        }

        _repositoryMock
            .Setup(r => r.ObterFolhaAnaliticaAsync(empresaId, "2026-05", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(itens);

        var query = new ObterFolhaAnaliticaQuery { EmpresaId = empresaId, Periodo = "2026-05", Page = 2, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.Funcionarios.Count); // página 2 com 10 itens
    }

    [Fact]
    public async Task Handle_QuandoNaoHaDados_DeveRetornarListaVazia()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ObterFolhaAnaliticaAsync(It.IsAny<Guid>(), It.IsAny<string>(), null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ItemFolhaView>());

        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Funcionarios);
    }
}
