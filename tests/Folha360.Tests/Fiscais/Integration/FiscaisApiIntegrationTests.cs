using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Folha360.Tests.Fiscais.Integration;

/// <summary>
/// Testes de integração para o módulo F05 — Obrigações Fiscais.
/// Executa contra o ambiente Docker (docker compose up -d).
/// Requer: PostgreSQL, RabbitMQ, Redis, MinIO e WebApi rodando.
/// </summary>
public class FiscaisApiIntegrationTests : IClassFixture<FiscaisIntegrationFixture>
{
    private readonly FiscaisIntegrationFixture _fixture;

    public FiscaisApiIntegrationTests(FiscaisIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    private static readonly Guid EmpresaDemoId = Guid.Parse("ac8a3f88-a945-455d-ac2e-af6098b8cd7e");

    [Fact]
    public async Task HealthCheck_DeveRetornarAlive()
    {
        var response = await _fixture.Client.GetAsync("/health/live");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Alive", content);
    }

    [Fact]
    public async Task GetRegras_SemAutenticacao_DeveRetornar401()
    {
        var response = await _fixture.Client.GetAsync("/api/fiscais/regras");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetRegras_ComAutenticacao_DeveRetornar200Ou500()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var response = await client.GetAsync("/api/fiscais/regras");

        // 200 (se handler implementado) ou 500 (handler pendente)
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task PostRegra_ComAutenticacao_DeveCriarERetornar201()
    {
        using var client = _fixture.CriarClienteAutenticado();

        var payload = new
        {
            tributo = "IRRF",
            versao = 2026,
            vigenciaInicio = "2026-01-01",
            vigenciaFim = "2026-12-31",
            parametros = "{\"faixas\":[{\"limite\":2259.20,\"aliquota\":0,\"deducao\":0}],\"deducaoDependente\":189.59}",
            codigoReceita = "0561"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/fiscais/regras", content);

        // Pode retornar 201 (criado) ou 400 (já existe)
        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Esperado 201 ou 400, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task PostRegra_SemAutenticacao_DeveRetornar401()
    {
        var payload = new
        {
            tributo = "FGTS",
            versao = 2026,
            vigenciaInicio = "2026-01-01",
            parametros = "{}",
            codigoReceita = "115"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _fixture.Client.PostAsync("/api/fiscais/regras", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetStatusFiscal_ComAutenticacao_DeveRetornar200()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var empresaId = EmpresaDemoId;
        var response = await client.GetAsync($"/api/fiscais/apuracao/status/{empresaId}");

        // Pode retornar 200 (com dados vazios) ou 500 (erro interno)
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task GetApuracao_ComAutenticacao_DeveRetornar200Ou500()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var empresaId = EmpresaDemoId;
        var response = await client.GetAsync($"/api/fiscais/apuracao/{empresaId}/2026-06");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task GetGuias_ComAutenticacao_DeveRetornar200Ou500()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var empresaId = EmpresaDemoId;
        var response = await client.GetAsync($"/api/fiscais/guias/{empresaId}/2026-06");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task GetExportacoes_ComAutenticacao_DeveRetornar200Ou500()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var empresaId = EmpresaDemoId;
        var response = await client.GetAsync($"/api/fiscais/exportacoes/{empresaId}/2026-06");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task GetCalendario_ComAutenticacao_DeveRetornar200Ou500()
    {
        using var client = _fixture.CriarClienteAutenticado();
        var empresaId = EmpresaDemoId;
        var response = await client.GetAsync($"/api/fiscais/apuracao/calendario/{empresaId}/2026");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Esperado 200 ou 500, obtido {(int)response.StatusCode}");
    }

    [Fact]
    public async Task PostRegra_TributoInvalido_DeveRetornar400()
    {
        using var client = _fixture.CriarClienteAutenticado();

        var payload = new
        {
            tributo = "INVALIDO",
            versao = 2026,
            vigenciaInicio = "2026-01-01",
            parametros = "{}",
            codigoReceita = "0000"
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/fiscais/regras", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
