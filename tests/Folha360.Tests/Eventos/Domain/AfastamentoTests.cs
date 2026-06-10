using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Entities;

namespace Folha360.Tests.Eventos.Domain;

[Trait("Category", "Unit")]
public class AfastamentoTests
{
    [Fact]
    public void Construtor_DeveCriarAfastamento_ComDadosValidos()
    {
        // Arrange
        var funcionarioId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var dataInicio = new DateOnly(2026, 6, 1);
        var dataFimPrevista = new DateOnly(2026, 6, 15);
        var tipoAfastamento = TipoAfastamento.Doenca;
        var cid = "J11.0";

        // Act
        var afastamento = new Afastamento(funcionarioId, empresaId, dataInicio, dataFimPrevista, tipoAfastamento, cid);

        // Assert
        Assert.NotEqual(Guid.Empty, afastamento.Id);
        Assert.Equal(funcionarioId, afastamento.FuncionarioId);
        Assert.Equal(empresaId, afastamento.EmpresaId);
        Assert.Equal(dataInicio, afastamento.DataInicio);
        Assert.Equal(dataFimPrevista, afastamento.DataFimPrevista);
        Assert.Equal(tipoAfastamento, afastamento.TipoAfastamento);
        Assert.Equal(cid, afastamento.Cid);
        Assert.Null(afastamento.DataFimEfetiva);
    }

    [Fact]
    public void Construtor_DeveCriarAfastamento_SemCid()
    {
        // Act
        var afastamento = new Afastamento(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), TipoAfastamento.LicencaMaternidade);

        // Assert
        Assert.Null(afastamento.Cid);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedades_EAtualizarUpdatedAt()
    {
        // Arrange
        var afastamento = new Afastamento(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 15), TipoAfastamento.Doenca, "J11.0");
        var updatedAtOriginal = afastamento.UpdatedAt;

        // Act
        Thread.Sleep(1);
        afastamento.Atualizar(new DateOnly(2026, 6, 1), new DateOnly(2026, 7, 15), TipoAfastamento.AcidenteTrabalho, new DateOnly(2026, 7, 10), "S62.0");

        // Assert
        Assert.True(afastamento.UpdatedAt > updatedAtOriginal);
        Assert.Equal(TipoAfastamento.AcidenteTrabalho, afastamento.TipoAfastamento);
        Assert.Equal(new DateOnly(2026, 7, 10), afastamento.DataFimEfetiva);
        Assert.Equal("S62.0", afastamento.Cid);
    }
}
