using Folha360.Domain;
using Folha360.Esocial.Domain.ValueObjects;

namespace Folha360.Esocial.Domain.Entities;

public class LoteEsocial : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public TipoAmbiente TipoAmbiente { get; private set; }
    public StatusLote Status { get; private set; }
    public string? ProtocoloEnvio { get; private set; }
    public string? ReciboGovernoJson { get; private set; }
    public int QuantidadeEventos { get; private set; }
    public DateTime? DataEnvio { get; private set; }
    public DateTime? DataProcessamento { get; private set; }

    private LoteEsocial()
    {
    }

    public LoteEsocial(Guid empresaId, TipoAmbiente tipoAmbiente)
    {
        EmpresaId = empresaId;
        TipoAmbiente = tipoAmbiente;
        Status = StatusLote.Pendente;
        QuantidadeEventos = 0;
    }

    public void IniciarAssinatura(int quantidadeEventos)
    {
        Status = StatusLote.Assinando;
        QuantidadeEventos = quantidadeEventos;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConcluirAssinatura()
    {
        Status = StatusLote.Assinado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Enviar(string protocoloEnvio)
    {
        if (string.IsNullOrWhiteSpace(protocoloEnvio))
            throw new ArgumentException("Protocolo de envio não pode ser vazio.", nameof(protocoloEnvio));
        Status = StatusLote.Enviado;
        ProtocoloEnvio = protocoloEnvio;
        DataEnvio = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Processar(string reciboGovernoJson)
    {
        Status = StatusLote.Processado;
        ReciboGovernoJson = reciboGovernoJson;
        DataProcessamento = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ProcessarParcialmente(string reciboGovernoJson)
    {
        Status = StatusLote.ParcialmenteProcessado;
        ReciboGovernoJson = reciboGovernoJson;
        DataProcessamento = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarcarErro()
    {
        Status = StatusLote.Erro;
        UpdatedAt = DateTime.UtcNow;
    }
}
