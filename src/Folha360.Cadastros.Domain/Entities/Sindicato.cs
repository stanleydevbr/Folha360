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

#pragma warning disable CS8618
    private Sindicato()
    {
    }
#pragma warning restore CS8618

    public Sindicato(
        Guid empresaId,
        string codigo,
        string nome,
        string? cnpj = null,
        string? tipo = null,
        decimal contribuicaoSindicalPercentual = 0,
        decimal contribuicaoAssistencialPercentual = 0)
    {
        if (contribuicaoSindicalPercentual < 0 || contribuicaoSindicalPercentual > 10)
            throw new ArgumentException("Contribuição sindical deve estar entre 0% e 10%.");

        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Nome = nome;
        Cnpj = cnpj;
        Tipo = tipo;
        ContribuicaoSindicalPercentual = contribuicaoSindicalPercentual;
        ContribuicaoAssistencialPercentual = contribuicaoAssistencialPercentual;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        string? cnpj = null,
        string? tipo = null,
        decimal? contribuicaoSindicalPercentual = null,
        decimal? contribuicaoAssistencialPercentual = null)
    {
        if (contribuicaoSindicalPercentual.HasValue &&
            (contribuicaoSindicalPercentual < 0 || contribuicaoSindicalPercentual > 10))
            throw new ArgumentException("Contribuição sindical deve estar entre 0% e 10%.");

        Nome = nome;
        Cnpj = cnpj;
        Tipo = tipo;
        ContribuicaoSindicalPercentual = contribuicaoSindicalPercentual ?? ContribuicaoSindicalPercentual;
        ContribuicaoAssistencialPercentual = contribuicaoAssistencialPercentual ?? ContribuicaoAssistencialPercentual;
        UpdatedAt = DateTime.UtcNow;
    }
}
