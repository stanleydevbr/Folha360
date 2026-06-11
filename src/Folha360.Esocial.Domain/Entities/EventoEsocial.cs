using Folha360.Domain;
using Folha360.Esocial.Domain.ValueObjects;

namespace Folha360.Esocial.Domain.Entities;

public class EventoEsocial : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public Guid? FuncionarioId { get; private set; }
    public TipoEventoEsocial TipoEvento { get; private set; }
    public string XmlConteudo { get; private set; }
    public StatusEvento Status { get; private set; }
    public Guid? LoteId { get; private set; }
    public string IdEvento { get; private set; }
    public Guid? CertificadoId { get; private set; }
    public string? HashAssinatura { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }

    private EventoEsocial()
    {
        XmlConteudo = string.Empty;
        IdEvento = string.Empty;
    }

    public EventoEsocial(
        Guid empresaId,
        TipoEventoEsocial tipoEvento,
        string xmlConteudo,
        string idEvento,
        Guid? funcionarioId = null)
        : this()
    {
        EmpresaId = empresaId;
        TipoEvento = tipoEvento;
        XmlConteudo = xmlConteudo;
        IdEvento = idEvento;
        FuncionarioId = funcionarioId;
        Status = StatusEvento.Pendente;
    }

    public void Validar()
    {
        if (Status != StatusEvento.Pendente)
            throw new InvalidOperationException($"Evento com status {Status} não pode ser validado.");
        Status = StatusEvento.Validado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Assinar(Guid certificadoId, string hashAssinatura)
    {
        if (Status != StatusEvento.Validado)
            throw new InvalidOperationException($"Evento com status {Status} não pode ser assinado.");
        CertificadoId = certificadoId;
        HashAssinatura = hashAssinatura;
        Status = StatusEvento.Assinado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Enviar(Guid loteId)
    {
        if (Status != StatusEvento.Assinado)
            throw new InvalidOperationException($"Evento com status {Status} não pode ser enviado.");
        LoteId = loteId;
        Status = StatusEvento.Enviado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Processar()
    {
        if (Status != StatusEvento.Enviado)
            throw new InvalidOperationException($"Evento com status {Status} não pode ser processado.");
        Status = StatusEvento.Processado;
        ProcessadoEm = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarcarErro()
    {
        Status = StatusEvento.Erro;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Retificar()
    {
        Status = StatusEvento.Retificado;
        UpdatedAt = DateTime.UtcNow;
    }
}
