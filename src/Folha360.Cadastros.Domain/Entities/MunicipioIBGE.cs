using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade MunicipioIBGE — Municípios brasileiros conforme IBGE.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class MunicipioIBGE : BaseEntity
{
    public string CodigoIbge { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public string Uf { get; private set; } = null!;
    public string CodigoUf { get; private set; } = null!;
    public bool Ativo { get; private set; } = true;

    private MunicipioIBGE()
        : base()
    {
    }

    public MunicipioIBGE(string codigoIbge, string nome, string uf, string codigoUf, bool ativo = true)
    {
        Id = Guid.NewGuid();
        CodigoIbge = codigoIbge ?? throw new ArgumentNullException(nameof(codigoIbge));
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Uf = uf ?? throw new ArgumentNullException(nameof(uf));
        CodigoUf = codigoUf ?? throw new ArgumentNullException(nameof(codigoUf));
        Ativo = ativo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string nome, string? uf = null, bool? ativo = null)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        if (uf != null)
        {
            Uf = uf;
        }

        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
