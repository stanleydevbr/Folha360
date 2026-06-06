using FluentValidation;
using Folha360.Eventos.Application.Commands;

namespace Folha360.Eventos.Application.Validators;

public class CriarFeriasCommandValidator : AbstractValidator<CriarFeriasCommand>
{
    public CriarFeriasCommandValidator()
    {
        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("Funcionário é obrigatório.");

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("Empresa é obrigatória.");

        RuleFor(x => x.DiasGozo)
            .InclusiveBetween(1, 60).WithMessage("Dias de gozo devem ser entre 1 e 60.");

        RuleFor(x => x.PeriodoAquisitivoFim)
            .GreaterThan(x => x.PeriodoAquisitivoInicio)
            .WithMessage("Período aquisitivo fim deve ser posterior ao início.");

        RuleFor(x => x.DataInicio)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(15)))
            .WithMessage("Férias devem ser concedidas com pelo menos 15 dias de antecedência.")
            .WithErrorCode("PRAZO_FERIAS_EXCEDIDO");
    }
}

public class AtualizarFeriasCommandValidator : AbstractValidator<AtualizarFeriasCommand>
{
    public AtualizarFeriasCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        RuleFor(x => x.DiasGozo)
            .InclusiveBetween(1, 60).WithMessage("Dias de gozo devem ser entre 1 e 60.");

        RuleFor(x => x.PeriodoAquisitivoFim)
            .GreaterThan(x => x.PeriodoAquisitivoInicio)
            .WithMessage("Período aquisitivo fim deve ser posterior ao início.");

        RuleFor(x => x.DataInicio)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(15)))
            .WithMessage("Férias devem ser concedidas com pelo menos 15 dias de antecedência.")
            .WithErrorCode("PRAZO_FERIAS_EXCEDIDO");
    }
}
