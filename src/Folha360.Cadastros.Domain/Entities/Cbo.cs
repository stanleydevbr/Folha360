using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade CboOcupacao — Classificação Brasileira de Ocupações (MTb).
/// Schema: public (compartilhado entre todos os tenants).
/// Não implementa ISoftDeletable — usa flag Ativo para desativação lógica.
/// </summary>
public class CboOcupacao : BaseEntity
{
    public string Codigo { get; private set; } = null!;
    public string Titulo { get; private set; } = null!;
    public bool Ativo { get; private set; } = true;

    private CboOcupacao()
        : base()
    {
    }

    public CboOcupacao(string codigo, string titulo, bool ativo = true)
    {
        Id = Guid.NewGuid();
        Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        Ativo = ativo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string titulo, bool? ativo = null)
    {
        Titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
