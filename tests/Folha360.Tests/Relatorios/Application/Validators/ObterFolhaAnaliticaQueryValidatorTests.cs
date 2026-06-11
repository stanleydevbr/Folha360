using Folha360.Relatorios.Application.Queries;
using Folha360.Relatorios.Application.Validators;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Relatorios.Application.Validators;

[Trait("Category", "Unit")]
public class ObterFolhaAnaliticaQueryValidatorTests
{
    private readonly ObterFolhaAnaliticaQueryValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenEmpresaIdIsEmpty()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.Empty, Periodo = "2026-05" };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EmpresaId);
    }

    [Fact]
    public void Should_HaveError_WhenPeriodoIsEmpty()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = string.Empty };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Periodo);
    }

    [Fact]
    public void Should_HaveError_WhenPeriodoExceedsMaxLength()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05-01" };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Periodo);
    }

    [Fact]
    public void Should_HaveError_WhenPageIsZeroOrNegative()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05", Page = 0 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeIsLessThanOne()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05", PageSize = 0 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_HaveError_WhenPageSizeExceeds100()
    {
        var query = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05", PageSize = 101 };
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_NotHaveError_WhenQueryIsValid()
    {
        var query = new ObterFolhaAnaliticaQuery
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            Page = 1,
            PageSize = 20,
        };
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_WithOptionalFilters()
    {
        var query = new ObterFolhaAnaliticaQuery
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            DepartamentoId = Guid.NewGuid(),
            TipoCalculo = "MENSAL",
            Page = 2,
            PageSize = 50,
        };
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_AcceptPageSizeAtBoundaries()
    {
        // PageSize = 1 (mínimo)
        var queryMin = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05", PageSize = 1 };
        Assert.True(_validator.TestValidate(queryMin).IsValid);

        // PageSize = 100 (máximo)
        var queryMax = new ObterFolhaAnaliticaQuery { EmpresaId = Guid.NewGuid(), Periodo = "2026-05", PageSize = 100 };
        Assert.True(_validator.TestValidate(queryMax).IsValid);
    }
}
