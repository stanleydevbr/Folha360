using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class GrupoRubrica : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string Natureza { get; private set; } = null!;
    public int OrdemExibicao { get; private set; }
    public ICollection<Rubrica> Rubricas { get; private set; } = new List<Rubrica>();

    private GrupoRubrica()
    {
    }

    public GrupoRubrica(Guid empresaId, string codigo, string descricao, string natureza, int ordemExibicao = 0)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Descricao = descricao;
        Natureza = natureza;
        OrdemExibicao = ordemExibicao;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string descricao, string natureza, int? ordemExibicao = null)
    {
        Descricao = descricao;
        Natureza = natureza;
        OrdemExibicao = ordemExibicao ?? OrdemExibicao;
        UpdatedAt = DateTime.UtcNow;
    }
}
