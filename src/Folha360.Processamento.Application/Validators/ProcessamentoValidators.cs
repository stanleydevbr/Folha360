using FluentValidation;
using Folha360.Processamento.Application.Commands;

namespace Folha360.Processamento.Application.Validators;

public class IniciarProcessamentoCommandValidator : AbstractValidator<IniciarProcessamentoCommand>
{
    public IniciarProcessamentoCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty().WithMessage("EmpresaId é obrigatório.");
        RuleFor(x => x.Periodo)
            .NotEmpty().WithMessage("Período é obrigatório.")
            .Must(p => DateOnly.TryParseExact(p, "yyyy-MM", out _))
            .WithMessage("Período deve estar no formato YYYY-MM.");
        RuleFor(x => x.TipoCalculo)
            .NotEmpty().WithMessage("Tipo de cálculo é obrigatório.");
    }
}

public class ReprocessarFolhaCommandValidator : AbstractValidator<ReprocessarFolhaCommand>
{
    public ReprocessarFolhaCommandValidator()
    {
        RuleFor(x => x.ProcessamentoId).NotEmpty().WithMessage("ProcessamentoId é obrigatório.");
    }
}

public class CancelarProcessamentoCommandValidator : AbstractValidator<CancelarProcessamentoCommand>
{
    public CancelarProcessamentoCommandValidator()
    {
        RuleFor(x => x.ProcessamentoId).NotEmpty().WithMessage("ProcessamentoId é obrigatório.");
    }
}
