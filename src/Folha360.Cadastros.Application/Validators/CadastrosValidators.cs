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
    private static readonly string[] NaturezasValidas =
    {
        "Vencimento", "Desconto", "Beneficio", "Informativo", "Provisao", "Base", "Complemento", "Reembolso", "Estagio"
    };

    private static readonly string[] TiposCalculoValidos =
    {
        "VALOR_FIXO", "PERCENTUAL", "HORA", "FORMULA", "COMPOSICAO", "TABELA_PROGRESSIVA", "UNIDADE", "DIA", "MEDIA", "TETO", "CONDICIONAL"
    };

    public CriarRubricaCommandValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(200);

        RuleFor(x => x.Natureza)
            .NotEmpty().WithMessage("Natureza é obrigatória.")
            .Must(x => NaturezasValidas.Contains(x))
            .WithMessage($"Natureza deve ser uma das: {string.Join(", ", NaturezasValidas)}");

        RuleFor(x => x.TipoCalculo)
            .NotEmpty().WithMessage("Tipo de cálculo é obrigatório.")
            .Must(x => TiposCalculoValidos.Contains(x))
            .WithMessage($"Tipo de cálculo deve ser um dos: {string.Join(", ", TiposCalculoValidos)}");

        RuleFor(x => x.RubricaBaseId)
            .NotNull().When(x => x.TipoCalculo == "PERCENTUAL")
            .WithMessage("Rubrica base é obrigatória quando tipo de cálculo é PERCENTUAL.");

        RuleFor(x => x.FormulaCalculo)
            .NotEmpty().When(x => x.TipoCalculo == "FORMULA")
            .WithMessage("Fórmula de cálculo é obrigatória quando tipo de cálculo é FORMULA.");

        RuleFor(x => x)
            .Must(x => !x.TetoMaximo.HasValue || !x.PisoMinimo.HasValue || x.TetoMaximo >= x.PisoMinimo)
            .WithMessage("Teto máximo não pode ser inferior ao piso mínimo.");

        RuleFor(x => x)
            .Must(x => !x.DataInicioVigencia.HasValue || !x.DataFimVigencia.HasValue || x.DataFimVigencia >= x.DataInicioVigencia)
            .WithMessage("Data fim de vigência não pode ser anterior à data início.");

        RuleFor(x => x.OrdemCalculo)
            .InclusiveBetween(0, 399).WithMessage("Ordem de cálculo deve estar entre 0 e 399.");
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
