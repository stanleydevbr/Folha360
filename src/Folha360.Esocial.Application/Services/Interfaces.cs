using System.Xml.Schema;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Entities;

namespace Folha360.Esocial.Application.Services;

public interface IXmlAssinaturaService
{
    Task<string> AssinarXmlAsync(string xml, CertificadoDigital certificado, string? senhaPin, CancellationToken ct);
}

public interface IEsocialEnvioService
{
    Task<string> EnviarLoteAsync(LoteEsocial lote, List<EventoEsocial> eventos, CancellationToken ct);
    Task<string> ConsultarReciboAsync(string protocolo, TipoAmbiente ambiente, CancellationToken ct);
}

public interface IXsdSchemaService
{
    Task<XmlSchemaSet> ObterSchemaAsync(TipoEventoEsocial tipoEvento, CancellationToken ct);
    Task AtualizarSchemasAsync(CancellationToken ct);
}

public interface ICertificadoService
{
    Task<System.Security.Cryptography.X509Certificates.X509Certificate2> CarregarCertificadoAsync(CertificadoDigital certificado, string? senhaPin, CancellationToken ct);
    Task<CertificadoInfo> ObterInfoAsync(CertificadoDigital certificado, CancellationToken ct);
}

public record CertificadoInfo(
    string Tipo,
    string Cnpj,
    string Emitente,
    DateTime DataExpiracao,
    int DiasRestantes,
    bool Expirado);
