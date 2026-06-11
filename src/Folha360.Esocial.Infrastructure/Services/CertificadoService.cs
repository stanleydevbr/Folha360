using System.Security.Cryptography.X509Certificates;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Folha360.Esocial.Infrastructure.Services;

public class CertificadoService : ICertificadoService
{
    private readonly ILogger<CertificadoService> _logger;

    public CertificadoService(ILogger<CertificadoService> logger)
    {
        _logger = logger;
    }

    public async Task<X509Certificate2> CarregarCertificadoAsync(CertificadoDigital certificado, string? senhaPin, CancellationToken ct)
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
                throw new InvalidOperationException($"Certificado A3 não encontrado: {certificado.Emitente}");

            return await Task.FromResult(certs[0]);
        }

        throw new NotSupportedException($"Tipo de certificado não suportado: {certificado.Tipo}");
    }

    public async Task<CertificadoInfo> ObterInfoAsync(CertificadoDigital certificado, CancellationToken ct)
    {
        return await Task.FromResult(new CertificadoInfo(
            Tipo: certificado.Tipo.ToString(),
            Cnpj: certificado.Cnpj,
            Emitente: certificado.Emitente,
            DataExpiracao: certificado.DataExpiracao,
            DiasRestantes: certificado.DiasRestantes,
            Expirado: certificado.EstaExpirado));
    }
}
