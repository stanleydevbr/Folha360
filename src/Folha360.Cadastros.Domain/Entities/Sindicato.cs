using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Sindicato — sindicatos e associações de classe para contribuição sindical.
/// Schema: tenant.
/// </summary>
public class Sindicato : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public string? Cnpj { get; private set; }
    public string? Tipo { get; private set; }
    public decimal ContribuicaoSindicalPercentual { get; private set; }
    public decimal ContribuicaoAssistencialPercentual { get; private set; }

    private Sindicato()
    {
    }

    public Sindicato(
        Guid empresaId,
        string codigo,
        string nome,
        string? cnpj = null,
        string? tipo = null,
        decimal contribuicaoSindicalPercentual = 0,
        decimal contribuicaoAssistencialPercentual = 0)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Nome = nome;
        Cnpj = StripNonDigits(cnpj);
        Tipo = tipo;
        ContribuicaoSindicalPercentual = contribuicaoSindicalPercentual;
        ContribuicaoAssistencialPercentual = contribuicaoAssistencialPercentual;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Atualizar(
        string nome,
        string? cnpj = null,
        string? tipo = null,
        decimal? contribuicaoSindicalPercentual = null,
        decimal? contribuicaoAssistencialPercentual = null)
    {
        Nome = nome;
        Cnpj = StripNonDigits(cnpj);
        Tipo = tipo;
        ContribuicaoSindicalPercentual = contribuicaoSindicalPercentual ?? ContribuicaoSindicalPercentual;
        ContribuicaoAssistencialPercentual = contribuicaoAssistencialPercentual ?? ContribuicaoAssistencialPercentual;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public override void Validate()
    {
        if (ContribuicaoSindicalPercentual < 0 || ContribuicaoSindicalPercentual > 10)
        {
            Notification.AddError("CONTRIBUICAO_INVALIDA",
                "Contribuição sindical deve estar entre 0% e 10%.",
                nameof(ContribuicaoSindicalPercentual));
        }
    }
}
