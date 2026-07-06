using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade ConfiguracaoGeral — configurações chave-valor por empresa.
/// Schema: tenant. Não implementa ISoftDeletable (remoção direta).
/// </summary>
public class ConfiguracaoGeral : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Chave { get; private set; } = null!;
    public string Valor { get; private set; } = null!;

    private ConfiguracaoGeral()
        : base()
    {
    }

    public ConfiguracaoGeral(Guid empresaId, string chave, string valor)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Chave = chave ?? throw new ArgumentNullException(nameof(chave));
        Valor = valor ?? throw new ArgumentNullException(nameof(valor));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarValor(string valor)
    {
        Valor = valor ?? throw new ArgumentNullException(nameof(valor));
        UpdatedAt = DateTime.UtcNow;
    }
}
