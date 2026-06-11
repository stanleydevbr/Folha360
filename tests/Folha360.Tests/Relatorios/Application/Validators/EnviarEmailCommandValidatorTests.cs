using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Validators;
using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Relatorios.Application.Validators;

[Trait("Category", "Unit")]
public class EnviarEmailCommandValidatorTests
{
    private readonly EnviarEmailCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenEmpresaIdIsEmpty()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.Empty,
            Periodo = "2026-05",
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EmpresaId);
    }

    [Fact]
    public void Should_HaveError_WhenPeriodoIsEmpty()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = string.Empty,
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Periodo);
    }

    [Fact]
    public void Should_HaveError_WhenDestinatariosIsEmpty()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            Destinatarios = new List<string>(),
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Destinatarios);
    }

    [Fact]
    public void Should_HaveError_WhenDestinatarioEmailIsInvalid()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            Destinatarios = new List<string> { "not-an-email" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("Destinatarios[0]");
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Periodo = "2026-05",
            Formato = FormatoExportacao.Pdf,
            Destinatarios = new List<string> { "contador@empresa.com.br" },
            Assunto = "Folha Analítica - Maio 2026",
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveError_WithMultipleValidDestinatarios()
    {
        var command = new EnviarEmailCommand
        {
            EmpresaId = Guid.NewGuid(),
            Periodo = "2026-05",
            Destinatarios = new List<string> { "a@test.com", "b@test.com", "c@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
