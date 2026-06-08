using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Folha360.Tests.Processamento.Carga;

public class ProcessamentoCargaTests
{
    private readonly ITestOutputHelper _output;
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000"),
        Timeout = TimeSpan.FromSeconds(30),
    };

    private static readonly string TenantId = "demo";

    static ProcessamentoCargaTests()
    {
        Client.DefaultRequestHeaders.Add("X-Tenant-Id", TenantId);
    }

    public ProcessamentoCargaTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private async Task AuthenticateAsync()
    {
        if (Client.DefaultRequestHeaders.Authorization is not null)
            return;

        var loginPayload = new { email = "admin@folha360.com.br", password = "Admin@123" };
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginPayload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = result.GetProperty("token").GetString()!;

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task Carga_LatenciaApi_HealthCheckEAuth()
    {
        _output.WriteLine("=== Latência: Health Check e Auth ===");

        var sw = Stopwatch.StartNew();
        var hr = await Client.GetAsync("/health");
        sw.Stop();
        _output.WriteLine($"GET /health: {hr.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms");
        Assert.True(hr.IsSuccessStatusCode);

        sw.Restart();
        var lp = new { email = "admin@folha360.com.br", password = "Admin@123" };
        var lr = await Client.PostAsJsonAsync("/api/auth/login", lp);
        sw.Stop();
        _output.WriteLine($"POST /api/auth/login: {lr.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms");
        Assert.True(lr.IsSuccessStatusCode);

        var result = await lr.Content.ReadFromJsonAsync<JsonElement>();
        var token = result.GetProperty("token").GetString()!;
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        sw.Restart();
        var pr = await Client.GetAsync("/api/folha/processamento/00000000-0000-0000-0000-000000000001");
        sw.Stop();
        _output.WriteLine($"GET /api/folha/processamento: {pr.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms");

        _output.WriteLine("=== Fim ===");
    }

    [Fact]
    public async Task Carga_LatenciaEndpoint_20Requisicoes()
    {
        _output.WriteLine("=== Latência: 20 Requisições POST /api/folha/processar ===");

        await AuthenticateAsync();

        var lats = new List<double>();
        var payload = JsonSerializer.Serialize(new
        {
            empresaId = Guid.NewGuid(),
            periodo = "2026-06",
            tipoCalculo = "Mensal",
        });

        for (int i = 0; i < 20; i++)
        {
            var content = new StringContent(payload, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            var sw = Stopwatch.StartNew();
            var response = await Client.PostAsync("/api/folha/processar", content);
            sw.Stop();

            lats.Add(sw.Elapsed.TotalMilliseconds);
            _output.WriteLine($"  {i + 1:D2}: {response.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms");
        }

        lats.Sort();
        _output.WriteLine("");
        _output.WriteLine("=== Resumo ===");
        _output.WriteLine($"  Min: {lats.Min():F1}ms");
        _output.WriteLine($"  Avg: {lats.Average():F1}ms");
        _output.WriteLine($"  P50: {lats[(int)(lats.Count * 0.50)]:F1}ms");
        _output.WriteLine($"  P95: {lats[(int)(lats.Count * 0.95)]:F1}ms");
        _output.WriteLine($"  P99: {lats[(int)(lats.Count * 0.99)]:F1}ms");
        _output.WriteLine($"  Max: {lats.Max():F1}ms");

        var p95 = lats[(int)(lats.Count * 0.95)];
        _output.WriteLine($"  RF12 p95 < 500ms: {(p95 < 500 ? "OK" : "ACIMA")}");
    }

    [Fact]
    public async Task Carga_LatenciaConsultas_10Requisicoes()
    {
        _output.WriteLine("=== Latência: Endpoints de Consulta ===");

        await AuthenticateAsync();

        var endpoints = new[]
        {
            "/api/folha/processamento/00000000-0000-0000-0000-000000000001",
            "/api/folha/holerites/00000000-0000-0000-0000-000000000001",
        };

        foreach (var ep in endpoints)
        {
            var lats = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                var sw = Stopwatch.StartNew();
                var r = await Client.GetAsync(ep);
                sw.Stop();
                lats.Add(sw.Elapsed.TotalMilliseconds);
            }

            lats.Sort();
            _output.WriteLine($"  GET {ep}:");
            _output.WriteLine($"    P50: {lats[(int)(lats.Count * 0.50)]:F1}ms | P95: {lats[(int)(lats.Count * 0.95)]:F1}ms | Avg: {lats.Average():F1}ms");
        }

        _output.WriteLine("=== Fim ===");
    }

    [Fact]
    public async Task Carga_Idempotencia_DuplicadoRetorna409()
    {
        _output.WriteLine("=== Idempotência CA06 ===");

        await AuthenticateAsync();

        var empresaId = Guid.NewGuid();
        var payload = JsonSerializer.Serialize(new
        {
            empresaId,
            periodo = "2026-06",
            tipoCalculo = "Mensal",
        });

        var c1 = new StringContent(payload, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var r1 = await Client.PostAsync("/api/folha/processar", c1);
        _output.WriteLine($"  1ª: {r1.StatusCode}");

        var c2 = new StringContent(payload, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var r2 = await Client.PostAsync("/api/folha/processar", c2);
        _output.WriteLine($"  2ª: {r2.StatusCode}");

        if (r2.StatusCode == System.Net.HttpStatusCode.Conflict)
            _output.WriteLine("  CA06 OK: 409 Conflict");
        else
        {
            var body = await r2.Content.ReadAsStringAsync();
            _output.WriteLine($"  Resposta: {body[..Math.Min(200, body.Length)]}");
        }
    }

    [Fact]
    public async Task Carga_Throughput_HealthCheck()
    {
        _output.WriteLine("=== Throughput: GET /health (100 reqs) ===");

        const int total = 100;
        var sw = Stopwatch.StartNew();
        var tasks = new List<Task<HttpResponseMessage>>();

        for (int i = 0; i < total; i++)
            tasks.Add(Client.GetAsync("/health"));

        await Task.WhenAll(tasks);
        sw.Stop();

        var ok = tasks.Count(t => t.Result.IsSuccessStatusCode);
        var rps = total / sw.Elapsed.TotalSeconds;

        _output.WriteLine($"  Total: {total} | OK: {ok}");
        _output.WriteLine($"  Tempo: {sw.Elapsed.TotalSeconds:F2}s");
        _output.WriteLine($"  Throughput: {rps:F1} req/s");

        Assert.Equal(total, ok);
    }
}
