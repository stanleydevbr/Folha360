using Folha360.Domain;

namespace Folha360.Esocial.Domain.Entities;

public class FalhaEsocial : BaseEntity
{
    public Guid EventoId { get; private set; }
    public Guid? LoteId { get; private set; }
    public TipoErroEsocial TipoErro { get; private set; }
    public string? CodigoErro { get; private set; }
    public string MensagemErro { get; private set; }
    public string? XmlOriginal { get; private set; }
    public int Tentativas { get; private set; }
    public DateTime DataUltimaTentativa { get; private set; }
    public DateTime? ResolvidoEm { get; private set; }

    private FalhaEsocial()
    {
        MensagemErro = string.Empty;
    }

    public FalhaEsocial(
        Guid eventoId,
        TipoErroEsocial tipoErro,
        string mensagemErro,
        Guid? loteId = null,
        string? codigoErro = null,
        string? xmlOriginal = null)
        : this()
    {
        EventoId = eventoId;
        TipoErro = tipoErro;
        MensagemErro = mensagemErro;
        LoteId = loteId;
        CodigoErro = codigoErro;
        XmlOriginal = xmlOriginal;
        Tentativas = 1;
        DataUltimaTentativa = DateTime.UtcNow;
    }

    public void IncrementarTentativa()
    {
        Tentativas++;
        DataUltimaTentativa = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolver()
    {
        ResolvidoEm = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
