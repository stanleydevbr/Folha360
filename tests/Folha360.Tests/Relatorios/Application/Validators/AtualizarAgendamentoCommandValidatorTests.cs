using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Validators;
using Folha360.Relatorios.Domain.Enums;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Relatorios.Application.Validators;

[Trait("Category", "Unit")]
public class AtualizarAgendamentoCommandValidatorTests
{
    private readonly AtualizarAgendamentoCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenAgendamentoIdIsEmpty()
    {
        var command = new AtualizarAgendamentoCommand { AgendamentoId = Guid.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AgendamentoId);
    }

    [Fact]
    public void Should_HaveError_WhenCronExpressionIsInvalid()
    {
        var command = new AtualizarAgendamentoCommand
        {
            AgendamentoId = Guid.NewGuid(),
            Recorrencia = "invalid-cron",
        };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Recorrencia);
    }

    [Fact]
    public void Should_NotHaveError_WhenRecorrenciaIsNull()
    {
        var command = new AtualizarAgendamentoCommand
        {
            AgendamentoId = Guid.NewGuid(),
            Recorrencia = null,
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Recorrencia);
    }

    [Fact]
    public void Should_NotHaveError_WhenRecorrenciaIsValidCron()
    {
        var command = new AtualizarAgendamentoCommand
        {
            AgendamentoId = Guid.NewGuid(),
            Recorrencia = "0 0 8 * * ?",
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Recorrencia);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new AtualizarAgendamentoCommand
        {
            AgendamentoId = Guid.NewGuid(),
            Recorrencia = "0 0 5 * * ?",
            Formato = FormatoExportacao.Csv,
            Destinatarios = new List<string> { "test@test.com" },
            Ativo = true,
        };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
