using FluentValidation;
using Folha360.Eventos.Application.Commands;

namespace Folha360.Eventos.Application.Validators;

public class CriarAdmissaoCommandValidator : AbstractValidator<CriarAdmissaoCommand>
{
    public CriarAdmissaoCommandValidator()
    {
        RuleFor(x => x.FuncionarioId)
            .NotEmpty().WithMessage("Funcionário é obrigatório.");

        RuleFor(x => x.EmpresaId)
            .NotEmpty().WithMessage("Empresa é obrigatória.");

        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("Cargo é obrigatório.");

        RuleFor(x => x.SalarioInicial)
            .GreaterThan(0).WithMessage("Salário inicial deve ser maior que zero.");

        RuleFor(x => x.DataAdmissao)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(-30)))
            .WithMessage("Admissão deve ser registrada em até 30 dias da data de início do vínculo.")
            .WithErrorCode("PRAZO_ADMISSAO_EXCEDIDO");
    }
}

public class AtualizarAdmissaoCommandValidator : AbstractValidator<AtualizarAdmissaoCommand>
{
    public AtualizarAdmissaoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("Cargo é obrigatório.");

        RuleFor(x => x.SalarioInicial)
            .GreaterThan(0).WithMessage("Salário inicial deve ser maior que zero.");

        RuleFor(x => x.DataAdmissao)
            .Must(data => data >= DateOnly.FromDateTime(DateTime.Today.AddDays(-30)))
            .WithMessage("Admissão deve ser registrada em até 30 dias da data de início do vínculo.")
            .WithErrorCode("PRAZO_ADMISSAO_EXCEDIDO");
    }
}
