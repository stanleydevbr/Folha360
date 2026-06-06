using FluentValidation;
using Folha360.Eventos.Application.Commands;

namespace Folha360.Eventos.Application.Validators;

public class CriarAlteracaoContratualCommandValidator : AbstractValidator<CriarAlteracaoContratualCommand>
{
    public CriarAlteracaoContratualCommandValidator()
    {
        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("Funcionário é obrigatório.");

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("Empresa é obrigatória.");

        RuleFor(x => x.DataAlteracao)
            .NotEmpty().WithMessage("Data da alteração é obrigatória.");
    }
}

public class AtualizarAlteracaoContratualCommandValidator : AbstractValidator<AtualizarAlteracaoContratualCommand>
{
    public AtualizarAlteracaoContratualCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        RuleFor(x => x.DataAlteracao)
            .NotEmpty().WithMessage("Data da alteração é obrigatória.");
    }
}
