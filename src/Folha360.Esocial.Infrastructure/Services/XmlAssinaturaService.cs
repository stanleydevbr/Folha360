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
    private readonly ICertificadoService _certificadoService;
    private readonly ILogger<XmlAssinaturaService> _logger;

    public XmlAssinaturaService(ICertificadoService certificadoService, ILogger<XmlAssinaturaService> logger)
    {
        _certificadoService = certificadoService;
        _logger = logger;
    }

    public async Task<string> AssinarXmlAsync(string xml, CertificadoDigital certificado, string? senhaPin, CancellationToken ct)
    {
        try
        {
            var cert = await _certificadoService.CarregarCertificadoAsync(certificado, senhaPin, ct);

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
}
