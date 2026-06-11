using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Queries;
using Folha360.Relatorios.Application.Validators;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Relatorios.Application.Validators;

[Trait("Category", "Unit")]
public class GerarHoleritesLoteCommandValidatorTests
{
    private readonly GerarHoleritesLoteCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenEmpresaIdIsEmpty()
    {
        var command = new GerarHoleritesLoteCommand { EmpresaId = Guid.Empty, Periodo = "2026-05" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EmpresaId);
    }

    [Fact]
    public void Should_HaveError_WhenPeriodoIsEmpty()
    {
        var command = new GerarHoleritesLoteCommand { EmpresaId = Guid.NewGuid(), Periodo = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Periodo);
    }

    [Fact]
    public void Should_HaveError_WhenPeriodoExceedsMaxLength()
    {
        var command = new GerarHoleritesLoteCommand { EmpresaId = Guid.NewGuid(), Periodo = "2026-05-01" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Periodo);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new GerarHoleritesLoteCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            FuncionarioIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_WhenFuncionarioIdsIsNull()
    {
        var command = new GerarHoleritesLoteCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            FuncionarioIds = null,
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
