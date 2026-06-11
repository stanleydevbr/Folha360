using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Folha360.Esocial.Infrastructure.Services;

public class XmlAssinaturaService : IXmlAssinaturaService
{
    private readonly ILogger<XmlAssinaturaService> _logger;

    public XmlAssinaturaService(ILogger<XmlAssinaturaService> logger)
    {
        _logger = logger;
    }

    public async Task<string> AssinarXmlAsync(string xml, CertificadoDigital certificado, string? senhaPin, CancellationToken ct)
    {
        try
        {
            X509Certificate2? cert = await CarregarCertificadoAsync(certificado, senhaPin);

            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xml);

            var signedXml = new SignedXml(xmlDoc) { SigningKey = cert.GetRSAPrivateKey() };

            var reference = new Reference { Uri = string.Empty };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            _logger.LogInformation("XML assinado com sucesso. Certificado: {Cnpj}", certificado.Cnpj);
            return await Task.FromResult(xmlDoc.OuterXml);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao assinar XML. Certificado: {Cnpj}", certificado.Cnpj);
            throw;
        }
    }

    private async Task<X509Certificate2> CarregarCertificadoAsync(CertificadoDigital certificado, string? senhaPin)
    {
        if (certificado.Tipo == TipoCertificado.A1)
        {
            if (certificado.ArquivoPfx == null || certificado.ArquivoPfx.Length == 0)
                throw new InvalidOperationException("Arquivo PFX não encontrado para certificado A1.");

            if (string.IsNullOrWhiteSpace(senhaPin))
                throw new InvalidOperationException("Senha do certificado A1 é obrigatória.");

            return await Task.FromResult(new X509Certificate2(certificado.ArquivoPfx, senhaPin));
        }
        else if (certificado.Tipo == TipoCertificado.A3)
        {
            if (string.IsNullOrWhiteSpace(senhaPin))
                throw new InvalidOperationException("PIN do token A3 é obrigatório.");

            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(X509FindType.FindBySubjectName, certificado.Emitente, true);
            if (certs.Count == 0)
                throw new InvalidOperationException($"Certificado A3 não encontrado no store: {certificado.Emitente}");

            return await Task.FromResult(certs[0]);
        }

        throw new NotSupportedException($"Tipo de certificado não suportado: {certificado.Tipo}");
    }
}
