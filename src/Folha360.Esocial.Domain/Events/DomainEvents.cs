namespace Folha360.Esocial.Domain.Events;

public record LoteProcessadoEvent(
    Guid LoteId,
    Guid EmpresaId,
    string Periodo,
    string Protocolo,
    int QuantidadeEventos,
    int EventosProcessados,
    int EventosRejeitados,
    DateTime OcorridoEm);

public record EventoComErroEvent(
    Guid EventoId,
    TipoEventoEsocial TipoEvento,
    string CodigoErro,
    string MensagemErro,
    DateTime OcorridoEm);

public record CertificadoExpirandoEvent(
    Guid CertificadoId,
    int DiasRestantes,
    DateTime DataExpiracao,
    DateTime OcorridoEm);

public record CertificadoExpiradoEvent(
    Guid CertificadoId,
    DateTime DataExpiracao,
    DateTime OcorridoEm);

public record PrazoEnvioCriticoEvent(
    Guid EmpresaId,
    TipoEventoEsocial TipoEvento,
    int DiasRestantes,
    DateTime PrazoLimite,
    DateTime OcorridoEm);

public record EventosEsocialEnviadosEvent(
    Guid EmpresaId,
    string Periodo,
    Guid LoteId,
    DateTime OcorridoEm);
