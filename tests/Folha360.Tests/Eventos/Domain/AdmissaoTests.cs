using Folha360.Eventos.Domain;
using Folha360.Eventos.Domain.Entities;

namespace Folha360.Tests.Eventos.Domain;

public class AdmissaoTests
{
    [Fact]
    public void Construtor_DeveCriarAdmissao_ComDadosValidos()
    {
        // Arrange
        var funcionarioId = Guid.NewGuid();
        var empresaId = Guid.NewGuid();
        var cargoId = Guid.NewGuid();
        var dataAdmissao = new DateOnly(2026, 6, 1);
        var salarioInicial = 5000.00m;
        var tipoContrato = TipoContrato.Indeterminado;

        // Act
        var admissao = new Admissao(funcionarioId, empresaId, dataAdmissao, cargoId, salarioInicial, tipoContrato);

        // Assert
        Assert.NotEqual(Guid.Empty, admissao.Id);
        Assert.Equal(funcionarioId, admissao.FuncionarioId);
        Assert.Equal(empresaId, admissao.EmpresaId);
        Assert.Equal(dataAdmissao, admissao.DataAdmissao);
        Assert.Equal(cargoId, admissao.CargoId);
        Assert.Equal(salarioInicial, admissao.SalarioInicial);
        Assert.Equal(tipoContrato, admissao.TipoContrato);
        Assert.NotEqual(default, admissao.CreatedAt);
        Assert.NotEqual(default, admissao.UpdatedAt);
    }

    [Fact]
    public void Construtor_DeveCriarAdmissao_ComPeriodoExperiencia()
    {
        // Act
        var admissao = new Admissao(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), Guid.NewGuid(), 3000m, TipoContrato.Experiencia, 3);

        // Assert
        Assert.Equal(3, admissao.PeriodoExperienciaMeses);
    }

    [Fact]
    public void Atualizar_DeveAlterarPropriedades_EAtualizarUpdatedAt()
    {
        // Arrange
        var admissao = new Admissao(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), Guid.NewGuid(), 5000m, TipoContrato.Indeterminado);
        var updatedAtOriginal = admissao.UpdatedAt;

        // Act
        Thread.Sleep(1);
        admissao.Atualizar(new DateOnly(2026, 7, 1), Guid.NewGuid(), 6000m, TipoContrato.Determinado, 6);

        // Assert
        Assert.True(admissao.UpdatedAt > updatedAtOriginal);
        Assert.Equal(6000m, admissao.SalarioInicial);
        Assert.Equal(TipoContrato.Determinado, admissao.TipoContrato);
        Assert.Equal(6, admissao.PeriodoExperienciaMeses);
    }
}
