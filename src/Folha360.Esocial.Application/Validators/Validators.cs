using FluentValidation;
using Folha360.Esocial.Application.Commands;

namespace Folha360.Esocial.Application.Validators;

public class EnviarLoteCommandValidator : AbstractValidator<EnviarLoteCommand>
{
    public EnviarLoteCommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.TipoAmbiente).NotEmpty().Must(x => x is "Producao" or "Homologacao")
            .WithMessage("TipoAmbiente deve ser 'Producao' ou 'Homologacao'.");
    }
}

public class UploadCertificadoA1CommandValidator : AbstractValidator<UploadCertificadoA1Command>
{
    public UploadCertificadoA1CommandValidator()
    {
        RuleFor(x => x.EmpresaId).NotEmpty();
        RuleFor(x => x.ArquivoPfx).NotEmpty().Must(a => a.Length > 0)
            .WithMessage("Arquivo PFX é obrigatório.");
        RuleFor(x => x.Senha).NotEmpty().MinimumLength(4)
            .WithMessage("Senha do certificado é obrigatória (mínimo 4 caracteres).");
    }
}
