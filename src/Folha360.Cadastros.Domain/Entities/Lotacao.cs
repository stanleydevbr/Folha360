using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Lotacao — lotação tributária/departamento compatível com S-1020 do e-Social.
/// Schema: tenant.
/// </summary>
public class Lotacao : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string? TipoEsocial { get; private set; }

    private Lotacao()
    {
    }

    public Lotacao(Guid empresaId, string codigo, string descricao, string? tipoEsocial = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Descricao = descricao;
        TipoEsocial = tipoEsocial;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string codigo, string descricao, string? tipoEsocial = null)
    {
        Codigo = codigo;
        Descricao = descricao;
        TipoEsocial = tipoEsocial;
        UpdatedAt = DateTime.UtcNow;
    }
}
