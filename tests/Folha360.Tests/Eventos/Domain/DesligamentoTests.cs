using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Entities;

namespace Folha360.Tests.Eventos.Domain;

[Trait("Category", "Unit")]
public class DesligamentoTests
{
    [Fact]
    public void Construtor_DeveCriarDesligamento_ComDadosValidos()
    {
        // Arrange
        var funcionarioId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var dataDesligamento = new DateOnly(2026, 6, 30);
        var motivo = MotivoDesligamento.SemJustaCausa;

        // Act
        var desligamento = new Desligamento(funcionarioId, empresaId, dataDesligamento, motivo);

        // Assert
        Assert.NotEqual(Guid.Empty, desligamento.Id);
        Assert.Equal(funcionarioId, desligamento.FuncionarioId);
        Assert.Equal(empresaId, desligamento.EmpresaId);
        Assert.Equal(dataDesligamento, desligamento.DataDesligamento);
        Assert.Equal(motivo, desligamento.MotivoDesligamento);
        Assert.Null(desligamento.VerbasRescisorias);
    }

    [Fact]
    public void Construtor_DeveCriarDesligamento_ComVerbasRescisorias()
    {
        // Act
        var desligamento = new Desligamento(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 30), MotivoDesligamento.PedidoDemissao, "{\"saldo_salario\":1500,\"ferias_vencidas\":2000}");

        // Assert
        Assert.NotNull(desligamento.VerbasRescisorias);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedades_EAtualizarUpdatedAt()
    {
        // Arrange
        var desligamento = new Desligamento(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 30), MotivoDesligamento.SemJustaCausa);
        var updatedAtOriginal = desligamento.UpdatedAt;

        // Act
        Thread.Sleep(1);
        desligamento.Atualizar(new DateOnly(2026, 7, 15), MotivoDesligamento.AcordoMutuo, "{\"saldo_salario\":2000}");

        // Assert
        Assert.True(desligamento.UpdatedAt > updatedAtOriginal);
        Assert.Equal(MotivoDesligamento.AcordoMutuo, desligamento.MotivoDesligamento);
        Assert.Equal("{\"saldo_salario\":2000}", desligamento.VerbasRescisorias);
    }
}
