using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Folha360.Tests.Fiscais.Integration;

/// <summary>
/// Fixture para testes de integração do F05 contra o ambiente Docker.
/// Conecta na API em http://localhost:5000 e gerencia autenticação JWT.
/// </summary>
public class FiscaisIntegrationFixture : IAsyncLifetime
{
    private readonly HttpClient _client;
    private string? _adminToken;

    public FiscaisIntegrationFixture()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000"),
            Timeout = TimeSpan.FromSeconds(30),
        };
    }

    public HttpClient Client => _client;

    public async Task InitializeAsync()
    {
        // Aguardar API estar healthy
        var healthy = false;
        for (int i = 0; i < 10; i++)
        {
            try
            {
                var healthResponse = await _client.GetAsync("/health/live");
                if (healthResponse.IsSuccessStatusCode)
                {
                    healthy = true;
                    break;
                }
            }
            catch
            {
                await Task.Delay(2000);
            }
        }

        if (!healthy)
        {
            throw new InvalidOperationException("API não está healthy após 10 tentativas.");
        }

        // Obter token JWT para admin
        _adminToken = await ObterTokenAsync("admin@folha360.com.br", "Admin@123");
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
    }

    /// <summary>
    /// Obtém token JWT autenticando via endpoint de login.
    /// </summary>
    private async Task<string?> ObterTokenAsync(string email, string password)
    {
        var loginPayload = new { email, password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("token", out var tokenProp))
        {
            return tokenProp.GetString();
        }

        return null;
    }

    /// <summary>
    /// Cria um HttpClient autenticado com token JWT de admin.
    /// </summary>
    public HttpClient CriarClienteAutenticado(string? token = null)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5000"),
            Timeout = TimeSpan.FromSeconds(30),
        };

        var tokenUsar = token ?? _adminToken;
        if (tokenUsar != null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsar);
        }

        return client;
    }
}
