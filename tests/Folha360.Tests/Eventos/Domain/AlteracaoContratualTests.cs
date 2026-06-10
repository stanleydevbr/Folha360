using Folha360.Eventos.Domain.Entities;

namespace Folha360.Tests.Eventos.Domain;

[Trait("Category", "Unit")]
public class AlteracaoContratualTests
{
    [Fact]
    public void Construtor_DeveCriarAlteracaoContratual_ComDadosValidos()
    {
        // Arrange
        var funcionarioId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var dataAlteracao = new DateOnly(2026, 7, 1);
        var camposAlterados = "{\"salario\":true,\"cargo\":false}";
        var valorAnterior = "{\"salario\":5000}";
        var valorNovo = "{\"salario\":6000}";

        // Act
        var alteracao = new AlteracaoContratual(funcionarioId, empresaId, dataAlteracao, camposAlterados, valorAnterior, valorNovo);

        // Assert
        Assert.NotEqual(Guid.Empty, alteracao.Id);
        Assert.Equal(funcionarioId, alteracao.FuncionarioId);
        Assert.Equal(empresaId, alteracao.EmpresaId);
        Assert.Equal(dataAlteracao, alteracao.DataAlteracao);
        Assert.Equal(camposAlterados, alteracao.CamposAlterados);
        Assert.Equal(valorAnterior, alteracao.ValorAnterior);
        Assert.Equal(valorNovo, alteracao.ValorNovo);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedades_EAtualizarUpdatedAt()
    {
        // Arrange
        var alteracao = new AlteracaoContratual(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 7, 1));
        var updatedAtOriginal = alteracao.UpdatedAt;

        // Act
        Thread.Sleep(1);
        alteracao.Atualizar(new DateOnly(2026, 8, 1), "{\"jornada\":true}", "{\"jornada\":40}", "{\"jornada\":44}");

        // Assert
        Assert.True(alteracao.UpdatedAt > updatedAtOriginal);
        Assert.Equal(new DateOnly(2026, 8, 1), alteracao.DataAlteracao);
        Assert.Equal("{\"jornada\":true}", alteracao.CamposAlterados);
    }
}
