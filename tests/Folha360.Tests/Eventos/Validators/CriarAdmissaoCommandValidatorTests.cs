using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Validators;
using Folha360.Eventos.Domain;

namespace Folha360.Tests.Eventos.Validators;

[Trait("Category", "Unit")]
public class CriarAdmissaoCommandValidatorTests
{
    private readonly CriarAdmissaoCommandValidator _validator = new();

    [Fact]
    public void DeveSerValido_ComDadosDentroDoPrazo()
    {
        var cmd = new CriarAdmissaoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            CargoId = Guid.NewGuid(),
            DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
            SalarioInicial = 5000m,
            TipoContrato = TipoContrato.Indeterminado,
        };

        var result = _validator.Validate(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveSerInvalido_ComPrazoAdmissaoExcedido()
    {
        var cmd = new CriarAdmissaoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            CargoId = Guid.NewGuid(),
            DataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-31)),
            SalarioInicial = 5000m,
            TipoContrato = TipoContrato.Indeterminado,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "PRAZO_ADMISSAO_EXCEDIDO");
    }

    [Fact]
    public void DeveSerInvalido_ComCamposObrigatoriosVazios()
    {
        var cmd = new CriarAdmissaoCommand
        {
            FuncionarioId = Guid.Empty,
            EmpresaId = Guid.Empty,
            CargoId = Guid.Empty,
            DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
            SalarioInicial = 0,
            TipoContrato = TipoContrato.Indeterminado,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
    }
}
