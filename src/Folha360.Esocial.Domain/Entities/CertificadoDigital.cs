using Folha360.Domain;

namespace Folha360.Esocial.Domain.Entities;

public class CertificadoDigital : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public TipoCertificado Tipo { get; private set; }
    public byte[]? ArquivoPfx { get; private set; }
    public string? CaminhoToken { get; private set; }
    public long? SlotId { get; private set; }
    public string Emitente { get; private set; }
    public string Cnpj { get; private set; }
    public DateTime DataExpiracao { get; private set; }
    public bool Ativo { get; private set; }

    private CertificadoDigital()
    {
        Emitente = string.Empty;
        Cnpj = string.Empty;
    }

    public CertificadoDigital(
        Guid empresaId,
        TipoCertificado tipo,
        string emitente,
        string cnpj,
        DateTime dataExpiracao,
        byte[]? arquivoPfx = null,
        string? caminhoToken = null,
        long? slotId = null)
        : this()
    {
        EmpresaId = empresaId;
        Tipo = tipo;
        Emitente = emitente;
        Cnpj = cnpj;
        DataExpiracao = dataExpiracao;
        ArquivoPfx = arquivoPfx;
        CaminhoToken = caminhoToken;
        SlotId = slotId;
        Ativo = true;
    }

    public bool EstaExpirado => DateTime.UtcNow.Date > DataExpiracao.Date;

    public int DiasRestantes => (DataExpiracao.Date - DateTime.UtcNow.Date).Days;

    public bool EstaExpirandoEmBreve(int diasLimite = 30) =>
        !EstaExpirado && DiasRestantes <= diasLimite;

    public void Desativar()
    {
        Ativo = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
