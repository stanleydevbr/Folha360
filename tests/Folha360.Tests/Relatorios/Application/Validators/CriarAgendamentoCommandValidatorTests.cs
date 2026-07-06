using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Validators;
using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Relatorios.Application.Validators;

[Trait("Category", "Unit")]
public class CriarAgendamentoCommandValidatorTests
{
    private readonly CriarAgendamentoCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenEmpresaIdIsEmpty()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.Empty,
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EmpresaId);
    }

    [Fact]
    public void Should_HaveError_WhenTipoRelatorioIsInvalid()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = (TipoRelatorio)999,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TipoRelatorio);
    }

    [Fact]
    public void Should_HaveError_WhenFormatoIsInvalid()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = (FormatoExportacao)999,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Formato);
    }

    [Fact]
    public void Should_HaveError_WhenRecorrenciaIsEmpty()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = string.Empty,
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Recorrencia);
    }

    [Fact]
    public void Should_HaveError_WhenCronExpressionIsInvalid()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = "invalid-cron-expression",
            Destinatarios = new List<string> { "test@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Recorrencia);
    }

    [Fact]
    public void Should_HaveError_WhenDestinatariosIsEmpty()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string>(),
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Destinatarios);
    }

    [Fact]
    public void Should_HaveError_WhenDestinatarioEmailIsInvalid()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.FolhaAnalitica,
            Formato = FormatoExportacao.Pdf,
            Recorrencia = "0 0 5 * * ?",
            Destinatarios = new List<string> { "invalid-email", "valid@test.com" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("Destinatarios[0]");
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new CriarAgendamentoCommand
        {
            EmpresaId = Guid.NewGuid(),
            TipoRelatorio = TipoRelatorio.ResumoMensal,
            Formato = FormatoExportacao.Csv,
            Recorrencia = "0 0 10 * * ?",
            Destinatarios = new List<string> { "contador@empresa.com.br", "admin@empresa.com.br" },
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_AcceptValidCronExpressions()
    {
        var validCrons = new[]
        {
            "0 0 5 * * ?",       // todo dia 5 às 00:00
            "0 0 8 ? * MON",     // toda segunda às 08:00
            "0 30 10 L * ?",     // último dia do mês às 10:30
            "0 0 0 1 1 ?",       // 1º de janeiro à meia-noite
        };

        foreach (var cron in validCrons)
        {
            var command = new CriarAgendamentoCommand
            {
                EmpresaId = Guid.NewGuid(),
                TipoRelatorio = TipoRelatorio.FolhaAnalitica,
                Formato = FormatoExportacao.Pdf,
                Recorrencia = cron,
                Destinatarios = new List<string> { "test@test.com" },
            };
            var result = _validator.TestValidate(command);
            Assert.True(result.IsValid, $"Cron '{cron}' deveria ser válida");
        }
    }
}
