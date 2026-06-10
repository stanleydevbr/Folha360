using FluentValidation;
using Folha360.Processamento.Application.Commands;

namespace Folha360.Processamento.Application.Validators;

public class ReabrirProcessamentoCommandValidator : AbstractValidator<ReabrirProcessamentoCommand>
{
    public ReabrirProcessamentoCommandValidator()
    {
        RuleFor(x => x.ProcessamentoId).NotEmpty().WithMessage("ProcessamentoId é obrigatório.");
        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo é obrigatório.")
            .MinimumLength(20).WithMessage("Motivo deve ter no mínimo 20 caracteres.");
        RuleFor(x => x.Autor).NotEmpty().WithMessage("Autor é obrigatório.");
    }
}
