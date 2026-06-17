using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade NaturezaJuridica — Tabela 21 do e-Social.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class NaturezaJuridica : BaseEntity
{
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public bool Ativo { get; private set; } = true;

    private NaturezaJuridica()
        : base()
    {
    }

    public NaturezaJuridica(string codigo, string descricao, bool ativo = true)
    {
        Id = Guid.NewGuid();
        Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
        Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
        Ativo = ativo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string descricao, bool? ativo = null)
    {
        Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
