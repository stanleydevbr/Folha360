using FluentValidation;

namespace Folha360.Cadastros.Application.Validators;

public class CriarEmpresaCommandValidator : AbstractValidator<Commands.CriarEmpresaCommand>
{
    public CriarEmpresaCommandValidator()
    {
        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório.")
            .Length(14).WithMessage("CNPJ deve ter 14 dígitos.");

        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória.")
            .MaximumLength(200).WithMessage("Razão Social deve ter no máximo 200 caracteres.");

        RuleFor(x => x.RegimeTributario)
            .NotEmpty().WithMessage("Regime Tributário é obrigatório.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("E-mail inválido.");
    }
}

public class CriarFuncionarioCommandValidator : AbstractValidator<Commands.CriarFuncionarioCommand>
{
    public CriarFuncionarioCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200);

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Length(11).WithMessage("CPF deve ter 11 dígitos.");

        RuleFor(x => x.DataAdmissao)
            .NotEmpty().WithMessage("Data de admissão é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Data de admissão não pode ser futura.");

        RuleFor(x => x.SalarioBase)
            .GreaterThan(0).WithMessage("Salário base deve ser maior que zero.");

        RuleFor(x => x.CargoId)
            .NotEmpty().WithMessage("Cargo é obrigatório.");

        RuleFor(x => x.LotacaoId)
            .NotEmpty().WithMessage("Lotação é obrigatória.");
    }
}

public class CriarCargoCommandValidator : AbstractValidator<Commands.CriarCargoCommand>
{
    public CriarCargoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200);

        RuleFor(x => x.Cbo)
            .NotEmpty().WithMessage("CBO é obrigatório.")
            .Length(6).WithMessage("CBO deve ter 6 dígitos.");

        RuleFor(x => x)
            .Must(x => !x.SalarioBaseMinimo.HasValue || !x.SalarioBaseMaximo.HasValue ||
                       x.SalarioBaseMinimo <= x.SalarioBaseMaximo)
            .WithMessage("Salário base mínimo não pode ser superior ao máximo.");
    }
}

public class CriarRubricaCommandValidator : AbstractValidator<Commands.CriarRubricaCommand>
{
    public CriarRubricaCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(200);

        RuleFor(x => x.Natureza)
            .NotEmpty().WithMessage("Natureza é obrigatória.");
    }
}

public class CriarLotacaoCommandValidator : AbstractValidator<Commands.CriarLotacaoCommand>
{
    public CriarLotacaoCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(200);
    }
}
