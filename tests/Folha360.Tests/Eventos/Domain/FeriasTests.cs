using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Entities;

namespace Folha360.Tests.Eventos.Domain;

public class FeriasTests
{
    [Fact]
    public void Construtor_DeveCriarFerias_ComDadosValidos()
    {
        // Arrange
        var funcionarioId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var dataInicio = new DateOnly(2026, 12, 1);
        var diasGozo = 30;
        var periodoInicio = new DateOnly(2025, 6, 1);
        var periodoFim = new DateOnly(2026, 5, 31);
        var tipoFerias = TipoFerias.Normais;

        // Act
        var ferias = new Ferias(funcionarioId, empresaId, dataInicio, diasGozo, periodoInicio, periodoFim, tipoFerias);

        // Assert
        Assert.NotEqual(Guid.Empty, ferias.Id);
        Assert.Equal(funcionarioId, ferias.FuncionarioId);
        Assert.Equal(empresaId, ferias.EmpresaId);
        Assert.Equal(dataInicio, ferias.DataInicio);
        Assert.Equal(diasGozo, ferias.DiasGozo);
        Assert.Equal(periodoInicio, ferias.PeriodoAquisitivoInicio);
        Assert.Equal(periodoFim, ferias.PeriodoAquisitivoFim);
        Assert.Equal(tipoFerias, ferias.TipoFerias);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedades_EAtualizarUpdatedAt()
    {
        // Arrange
        var ferias = new Ferias(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 12, 1), 30, new DateOnly(2025, 6, 1), new DateOnly(2026, 5, 31), TipoFerias.Normais);
        var updatedAtOriginal = ferias.UpdatedAt;

        // Act
        Thread.Sleep(1);
        ferias.Atualizar(new DateOnly(2027, 1, 15), 20, new DateOnly(2026, 6, 1), new DateOnly(2027, 5, 31), TipoFerias.Coletivas);

        // Assert
        Assert.True(ferias.UpdatedAt > updatedAtOriginal);
        Assert.Equal(20, ferias.DiasGozo);
        Assert.Equal(TipoFerias.Coletivas, ferias.TipoFerias);
    }
}
