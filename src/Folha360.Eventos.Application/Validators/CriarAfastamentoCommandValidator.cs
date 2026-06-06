using FluentValidation;
using Folha360.Eventos.Application.Commands;

namespace Folha360.Eventos.Application.Validators;

public class CriarAfastamentoCommandValidator : AbstractValidator<CriarAfastamentoCommand>
{
    public CriarAfastamentoCommandValidator()
    {
        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("Funcionário é obrigatório.");

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("Empresa é obrigatória.");

        RuleFor(x => x.DataInicio)
            .Must(data => data <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de início do afastamento não pode ser futura.")
            .WithErrorCode("DATA_INICIO_FUTURA");

        RuleFor(x => x.DataFimPrevista)
            .GreaterThan(x => x.DataInicio)
            .WithMessage("Data fim prevista deve ser posterior à data de início.")
            .WithErrorCode("DATA_FIM_INVALIDA");
    }
}

public class AtualizarAfastamentoCommandValidator : AbstractValidator<AtualizarAfastamentoCommand>
{
    public AtualizarAfastamentoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        RuleFor(x => x.DataInicio)
            .Must(data => data <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de início do afastamento não pode ser futura.")
            .WithErrorCode("DATA_INICIO_FUTURA");

        RuleFor(x => x.DataFimPrevista)
            .GreaterThan(x => x.DataInicio)
            .WithMessage("Data fim prevista deve ser posterior à data de início.")
            .WithErrorCode("DATA_FIM_INVALIDA");
    }
}
