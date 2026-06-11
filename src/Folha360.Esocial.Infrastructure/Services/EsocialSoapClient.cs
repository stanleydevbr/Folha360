using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Folha360.Esocial.Application.Services;
using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Folha360.Esocial.Infrastructure.Services;

public class EsocialSoapClient : IEsocialEnvioService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EsocialSoapClient> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    private static readonly Dictionary<TipoAmbiente, string> Endpoints = new()
    {
        [TipoAmbiente.Producao] = "https://webservices.envio.esocial.gov.br/servico/empregador/enviarloteeventos/WsEnviarLoteEventos.svc",
        [TipoAmbiente.Homologacao] = "https://webservices.producaorestrita.esocial.gov.br/servico/empregador/enviarloteeventos/WsEnviarLoteEventos.svc",
    };

    public EsocialSoapClient(HttpClient httpClient, IConfiguration configuration, ILogger<EsocialSoapClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(
                        "Tentativa {RetryCount} de envio SOAP falhou. Aguardando {Delay}s antes da próxima tentativa. Status: {StatusCode}",
                        retryCount, timeSpan.TotalSeconds, result.Result?.StatusCode);
                });
    }

    public async Task<string> EnviarLoteAsync(LoteEsocial lote, List<EventoEsocial> eventos, CancellationToken ct)
    {
        var endpoint = ObterEndpoint(lote.TipoAmbiente);
        var soapEnvelope = ConstruirEnvelopeEnvio(lote, eventos);

        _logger.LogInformation("Enviando lote {LoteId} com {Count} eventos para {Endpoint}", lote.Id, eventos.Count, endpoint);

        using var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml");
        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(endpoint, content, ct));
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var protocolo = ExtrairProtocolo(responseBody);

        _logger.LogInformation("Lote {LoteId} enviado com sucesso. Protocolo: {Protocolo}", lote.Id, protocolo);
        return protocolo;
    }

    public async Task<string> ConsultarReciboAsync(string protocolo, TipoAmbiente ambiente, CancellationToken ct)
    {
        var endpoint = ObterEndpoint(ambiente).Replace("enviarloteeventos", "consultarloteeventos");
        var soapEnvelope = ConstruirEnvelopeConsulta(protocolo);

        _logger.LogInformation("Consultando recibo do protocolo {Protocolo}", protocolo);

        using var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml");
        var response = await _retryPolicy.ExecuteAsync(() => _httpClient.PostAsync(endpoint, content, ct));
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        return responseBody;
    }

    private string ObterEndpoint(TipoAmbiente ambiente)
    {
        var configEndpoint = _configuration[$"Esocial:Endpoint:{ambiente}"];
        return configEndpoint ?? Endpoints[ambiente];
    }

    private static string ConstruirEnvelopeEnvio(LoteEsocial lote, List<EventoEsocial> eventos)
    {
        var eventosXml = string.Join(Environment.NewLine, eventos.Select(e => e.XmlConteudo));

        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope""
                 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <soap12:Body>
    <EnviarLoteEventos xmlns=""http://www.esocial.gov.br/servico/empregador/lote/eventos/envio/v1_1_0"">
      <loteEventos>
        {eventosXml}
      </loteEventos>
    </EnviarLoteEventos>
  </soap12:Body>
</soap12:Envelope>";
    }

    private static string ConstruirEnvelopeConsulta(string protocolo)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap12:Envelope xmlns:soap12=""http://www.w3.org/2003/05/soap-envelope""
                 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <soap12:Body>
    <ConsultarLoteEventos xmlns=""http://www.esocial.gov.br/servico/empregador/lote/eventos/envio/consulta/v1_1_0"">
      <protocoloEnvio>{protocolo}</protocoloEnvio>
    </ConsultarLoteEventos>
  </soap12:Body>
</soap12:Envelope>";
    }

    private static string ExtrairProtocolo(string soapResponse)
    {
        var doc = XDocument.Parse(soapResponse);
        var ns = XNamespace.Get("http://www.esocial.gov.br/servico/empregador/lote/eventos/envio/retornoEnvio/v1_1_0");
        var protocolo = doc.Descendants(ns + "protocoloEnvio").FirstOrDefault()?.Value;
        return protocolo ?? throw new InvalidOperationException("Protocolo não encontrado na resposta SOAP.");
    }
}
