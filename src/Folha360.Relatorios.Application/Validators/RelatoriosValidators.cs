namespace Folha360.Relatorios.Application.Validators;

public class GerarHoleritesLoteCommandValidator : AbstractValidator<GerarHoleritesLoteCommand>
{
    public GerarHoleritesLoteCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.Periodo).NotEmpty().MaximumLength(7);
    }
}

public class CriarAgendamentoCommandValidator : AbstractValidator<CriarAgendamentoCommand>
{
    public CriarAgendamentoCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.TipoRelatorio).IsInEnum();
        RuleFor(x => x.Formato).IsInEnum();
        RuleFor(x => x.Recorrencia).NotEmpty().Must(BeValidCron).WithMessage("Expressão cron inválida.");
        RuleFor(x => x.Destinatarios).NotEmpty();
        RuleForEach(x => x.Destinatarios).EmailAddress();
    }

    private static bool BeValidCron(string cron)
    {
        return Quartz.CronExpression.IsValidExpression(cron);
    }
}

public class AtualizarAgendamentoCommandValidator : AbstractValidator<AtualizarAgendamentoCommand>
{
    public AtualizarAgendamentoCommandValidator()
    {
        RuleFor(x => x.AgendamentoId).NotEmpty();
        RuleFor(x => x.Recorrencia)
            .Must(cron => cron is null || Quartz.CronExpression.IsValidExpression(cron))
            .WithMessage("Expressão cron inválida.");
    }
}

public class EnviarEmailCommandValidator : AbstractValidator<EnviarEmailCommand>
{
    public EnviarEmailCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.Periodo).NotEmpty();
        RuleFor(x => x.Destinatarios).NotEmpty();
        RuleForEach(x => x.Destinatarios).EmailAddress();
    }
}

public class ObterFolhaAnaliticaQueryValidator : AbstractValidator<ObterFolhaAnaliticaQuery>
{
    public ObterFolhaAnaliticaQueryValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.Periodo).NotEmpty().MaximumLength(7);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
