using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade BancoFebraban — Bancos brasileiros conforme Febraban.
/// Schema: public (compartilhado entre todos os tenants).
/// </summary>
public class BancoFebraban : BaseEntity
{
    public string Codigo { get; private set; } = null!;
    public string Nome { get; private set; } = null!;
    public bool Ativo { get; private set; } = true;

    private BancoFebraban()
        : base()
    {
    }

    public BancoFebraban(string codigo, string nome, bool ativo = true)
    {
        Id = Guid.NewGuid();
        Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Ativo = ativo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string nome, bool? ativo = null)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
