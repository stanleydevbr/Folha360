using FluentValidation;
using Folha360.Eventos.Application.Commands;

namespace Folha360.Eventos.Application.Validators;

public class CriarDesligamentoCommandValidator : AbstractValidator<CriarDesligamentoCommand>
{
    public CriarDesligamentoCommandValidator()
    {
        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("Funcionário é obrigatório.");

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("Empresa é obrigatória.");

        RuleFor(x => x.DataDesligamento)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(-10)))
            .WithMessage("Desligamento deve ser registrado em até 10 dias da data de desligamento.")
            .WithErrorCode("PRAZO_DESLIGAMENTO_EXCEDIDO");
    }
}

public class AtualizarDesligamentoCommandValidator : AbstractValidator<AtualizarDesligamentoCommand>
{
    public AtualizarDesligamentoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        RuleFor(x => x.DataDesligamento)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(-10)))
            .WithMessage("Desligamento deve ser registrado em até 10 dias da data de desligamento.")
            .WithErrorCode("PRAZO_DESLIGAMENTO_EXCEDIDO");
    }
}
