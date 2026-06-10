using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Validators;

namespace Folha360.Tests.Eventos.Validators;

[Trait("Category", "Unit")]
public class CriarAlteracaoContratualCommandValidatorTests
{
    private readonly CriarAlteracaoContratualCommandValidator _validator = new();

    [Fact]
    public void DeveSerValido_ComDadosCompletos()
    {
        var cmd = new CriarAlteracaoContratualCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataAlteracao = new DateOnly(2026, 7, 1),
            CamposAlterados = "{\"salario\":true}",
        };

        var result = _validator.Validate(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveSerInvalido_ComCamposObrigatoriosVazios()
    {
        var cmd = new CriarAlteracaoContratualCommand
        {
            FuncionarioId = Guid.Empty,
            EmpresaId = Guid.Empty,
            DataAlteracao = default,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
    }
}
