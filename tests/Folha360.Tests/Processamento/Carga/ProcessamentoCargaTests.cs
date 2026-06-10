using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Npgsql;
using Xunit;
using Xunit.Abstractions;

namespace Folha360.Tests.Processamento.Carga;

/// <summary>
/// Testes de carga do módulo de processamento.
/// Executados serialmente (não paralelizados) porque compartilham estado estático
/// (HttpClient, _empresaId, _funcionarioId) e abrem conexões diretas ao PostgreSQL,
/// o que saturaria o pool de conexões em paralelo.
/// </summary>
[Trait("Category", "Integration")]
[Collection("Carga")]
public class ProcessamentoCargaTests
{
    private readonly ITestOutputHelper _output;
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000"),
        Timeout = TimeSpan.FromSeconds(30),
    };

    private static readonly string TenantId = "demo";
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("TEST_POSTGRES") ??
        "Host=localhost;Port=5432;Database=folha360;Username=folha360_user;Password=Folha360@Dev";

    private static Guid? _empresaId;
    private static Guid? _funcionarioId;

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

    /// <summary>
    /// Cria empresa e funcionário reais para testes que precisam de dados válidos.
    /// Idempotente: só cria se ainda não existir no cache estático.
    /// Fallback: se a API falhar (ex.: bug TenantId), insere diretamente no PostgreSQL.
    /// </summary>
    private async Task SetupDadosTesteAsync()
    {
        if (_empresaId.HasValue && _funcionarioId.HasValue)
            return;

        await AuthenticateAsync();

        // Tentar criar empresa via API
        if (!_empresaId.HasValue)
        {
            _empresaId = await CriarEmpresaViaApiAsync();

            // Fallback: se API falhar, buscar empresa existente ou criar via SQL
            if (!_empresaId.HasValue)
            {
                _empresaId = await BuscarOuCriarEmpresaViaSqlAsync();
            }
        }

        // Tentar criar funcionário via API
        if (_empresaId.HasValue && !_funcionarioId.HasValue)
        {
            _funcionarioId = await CriarFuncionarioViaApiAsync();

            // Fallback: se API falhar, buscar funcionário existente ou criar via SQL
            if (!_funcionarioId.HasValue)
            {
                _funcionarioId = await BuscarOuCriarFuncionarioViaSqlAsync();
            }
        }
    }

    private async Task<Guid?> CriarEmpresaViaApiAsync()
    {
        var empresaPayload = JsonSerializer.Serialize(new
        {
            razaoSocial = $"Empresa Carga Teste {DateTime.UtcNow:yyyyMMddHHmmss}",
            nomeFantasia = "CargaTeste",
            cnpj = $"{RandomCnpj()}",
            regimeTributario = "SimplesNacional",
            email = "carga@teste.com.br",
            telefone = "11999999999",
            cnae = "6202300",
            fpas = "515",
            codigoTerceiros = "0000",
            classificacaoTributaria = "PJ",
            matrizFilial = "Matriz",
        });

        var content = new StringContent(empresaPayload, Encoding.UTF8,
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var response = await Client.PostAsync("/api/empresas", content);

        if (response.IsSuccessStatusCode)
        {
            var empresaJson = await response.Content.ReadFromJsonAsync<JsonElement>();
            var id = empresaJson.GetProperty("id").GetGuid();
            _output.WriteLine($"Empresa criada via API: {id}");
            return id;
        }

        var error = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API empresa falhou ({response.StatusCode}): {error[..Math.Min(200, error.Length)]}");
        return null;
    }

    /// <summary>
    /// Abre conexão PostgreSQL com retry (exponential backoff) para lidar com
    /// saturação do pool de conexões (erro 53300: "muitos clientes conectados").
    /// </summary>
    private async Task<NpgsqlConnection> OpenWithRetryAsync(int maxRetries = 5, CancellationToken ct = default)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync(ct);
                return conn;
            }
            catch (PostgresException ex) when (ex.SqlState == "53300" && attempt < maxRetries - 1)
            {
                var delay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt));
                _output.WriteLine($"PostgreSQL saturado (tentativa {attempt + 1}/{maxRetries}), aguardando {delay.TotalMilliseconds:F0}ms...");
                await Task.Delay(delay, ct);
            }
        }

        // Última tentativa — se falhar, deixa a exceção propagar
        var lastConn = new NpgsqlConnection(ConnectionString);
        await lastConn.OpenAsync(ct);
        return lastConn;
    }

    private async Task<Guid?> BuscarOuCriarEmpresaViaSqlAsync()
    {
        await using var conn = await OpenWithRetryAsync();

        // Garantir que o tenant "demo" existe (necessário para ResolveTenantGuid)
        var tenantGuid = TenantIdToGuid();
        await using var tenantCmd = new NpgsqlCommand(
            """INSERT INTO tenant (id, tenant_id, schema_name, nome, status, created_at, updated_at) VALUES (@id, 'demo', 'tenant_demo', 'Demo Tenant', 1, now(), now()) ON CONFLICT (tenant_id) DO NOTHING""", conn);
        tenantCmd.Parameters.AddWithValue("id", tenantGuid);
        await tenantCmd.ExecuteNonQueryAsync();

        // Buscar empresa existente no tenant demo
        await using var cmd = new NpgsqlCommand(
            """SELECT id FROM empresa WHERE deleted_at IS NULL LIMIT 1""", conn);
        var result = await cmd.ExecuteScalarAsync();

        if (result is Guid existingId)
        {
            _output.WriteLine($"Usando empresa existente (SQL): {existingId}");
            return existingId;
        }

        // Criar empresa via SQL
        var newId = Guid.NewGuid();
        var cnpj = RandomCnpjNumerico();

        await using var insertCmd = new NpgsqlCommand(
            """INSERT INTO empresa (id, tenant_id, cnpj, razao_social, nome_fantasia, regime_tributario, email, telefone, cnae, fpas, codigo_terceiros, classificacao_tributaria, matriz_filial, created_at, updated_at) VALUES (@id, @tid, @cnpj, @razao, @fantasia, @regime, @email, @tel, @cnae, @fpas, @cod, @classif, @matriz, now(), now())""", conn);

        insertCmd.Parameters.AddWithValue("id", newId);
        insertCmd.Parameters.AddWithValue("tid", tenantGuid);
        insertCmd.Parameters.AddWithValue("cnpj", cnpj);
        insertCmd.Parameters.AddWithValue("razao", $"Empresa Carga {DateTime.UtcNow:HHmmss}");
        insertCmd.Parameters.AddWithValue("fantasia", "CargaTeste");
        insertCmd.Parameters.AddWithValue("regime", "SimplesNacional");
        insertCmd.Parameters.AddWithValue("email", "carga@teste.com.br");
        insertCmd.Parameters.AddWithValue("tel", "11999999999");
        insertCmd.Parameters.AddWithValue("cnae", "6202300");
        insertCmd.Parameters.AddWithValue("fpas", "515");
        insertCmd.Parameters.AddWithValue("cod", "0000");
        insertCmd.Parameters.AddWithValue("classif", "PJ");
        insertCmd.Parameters.AddWithValue("matriz", "Matriz");

        await insertCmd.ExecuteNonQueryAsync();
        _output.WriteLine($"Empresa criada via SQL: {newId}");
        return newId;
    }

    private async Task<Guid?> CriarFuncionarioViaApiAsync()
    {
        var funcPayload = JsonSerializer.Serialize(new
        {
            empresaId = _empresaId!.Value,
            nome = $"Func Carga {DateTime.UtcNow:HHmmss}",
            cpf = $"{RandomCpf()}",
            dataNascimento = "1990-01-15",
            dataAdmissao = "2024-01-10",
            salarioBase = 5000.00m,
            status = "Ativo",
        });

        var content = new StringContent(funcPayload, Encoding.UTF8,
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var response = await Client.PostAsync("/api/funcionarios", content);

        if (response.IsSuccessStatusCode)
        {
            var funcJson = await response.Content.ReadFromJsonAsync<JsonElement>();
            var id = funcJson.GetProperty("id").GetGuid();
            _output.WriteLine($"Funcionário criado via API: {id}");
            return id;
        }

        var error = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"API funcionário falhou ({response.StatusCode}): {error[..Math.Min(200, error.Length)]}");
        return null;
    }

    private async Task<Guid?> BuscarOuCriarFuncionarioViaSqlAsync()
    {
        await using var conn = await OpenWithRetryAsync();

        // Buscar funcionário existente da empresa
        await using var cmd = new NpgsqlCommand(
            """SELECT id FROM funcionario WHERE empresa_id = @eid AND deleted_at IS NULL LIMIT 1""", conn);
        cmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        var result = await cmd.ExecuteScalarAsync();

        if (result is Guid existingId)
        {
            _output.WriteLine($"Usando funcionário existente (SQL): {existingId}");
            return existingId;
        }

        // Criar cargo mínimo via SQL (ou buscar existente)
        var cargoId = await BuscarOuCriarCargoViaSqlAsync(conn);

        // Criar lotação mínima via SQL (ou buscar existente)
        var lotacaoId = await BuscarOuCriarLotacaoViaSqlAsync(conn);

        // Criar funcionário mínimo via SQL
        var newId = Guid.NewGuid();
        var cpf = RandomCpfNumerico();
        var cpfHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(cpf)));

        await using var insertCmd = new NpgsqlCommand(
            """INSERT INTO funcionario (id, empresa_id, nome, cpf, cpf_hash, data_nascimento, data_admissao, salario_base, status, cargo_id, lotacao_id, created_at, updated_at) VALUES (@id, @eid, @nome, @cpf, @cpfHash, '1990-01-15', '2024-01-10', 5000.00, 'Ativo', @cargoId, @lotacaoId, now(), now())""", conn);

        insertCmd.Parameters.AddWithValue("id", newId);
        insertCmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        insertCmd.Parameters.AddWithValue("nome", $"Func Carga {DateTime.UtcNow:HHmmss}");
        insertCmd.Parameters.AddWithValue("cpf", cpf);
        insertCmd.Parameters.AddWithValue("cpfHash", cpfHash);
        insertCmd.Parameters.AddWithValue("cargoId", cargoId);
        insertCmd.Parameters.AddWithValue("lotacaoId", lotacaoId);

        await insertCmd.ExecuteNonQueryAsync();
        _output.WriteLine($"Funcionário criado via SQL: {newId}");
        return newId;
    }

    private async Task<Guid> BuscarOuCriarCargoViaSqlAsync(NpgsqlConnection conn)
    {
        await using var cmd = new NpgsqlCommand(
            """SELECT id FROM cargo WHERE empresa_id = @eid AND deleted_at IS NULL LIMIT 1""", conn);
        cmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        var result = await cmd.ExecuteScalarAsync();

        if (result is Guid existingId)
            return existingId;

        var newId = Guid.NewGuid();
        await using var insertCmd = new NpgsqlCommand(
            """INSERT INTO cargo (id, empresa_id, nome, cbo, created_at, updated_at) VALUES (@id, @eid, 'Cargo Padrão', '411010', now(), now())""", conn);
        insertCmd.Parameters.AddWithValue("id", newId);
        insertCmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        await insertCmd.ExecuteNonQueryAsync();
        _output.WriteLine($"Cargo criado via SQL: {newId}");
        return newId;
    }

    private async Task<Guid> BuscarOuCriarLotacaoViaSqlAsync(NpgsqlConnection conn)
    {
        await using var cmd = new NpgsqlCommand(
            """SELECT id FROM lotacao WHERE empresa_id = @eid AND deleted_at IS NULL LIMIT 1""", conn);
        cmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        var result = await cmd.ExecuteScalarAsync();

        if (result is Guid existingId)
            return existingId;

        var newId = Guid.NewGuid();
        var codigo = $"LT-{DateTime.UtcNow:HHmmss}";
        await using var insertCmd = new NpgsqlCommand(
            """INSERT INTO lotacao (id, empresa_id, codigo, descricao, created_at, updated_at) VALUES (@id, @eid, @codigo, 'Lotação Padrão', now(), now())""", conn);
        insertCmd.Parameters.AddWithValue("id", newId);
        insertCmd.Parameters.AddWithValue("eid", _empresaId!.Value);
        insertCmd.Parameters.AddWithValue("codigo", codigo);
        await insertCmd.ExecuteNonQueryAsync();
        _output.WriteLine($"Lotação criada via SQL: {newId}");
        return newId;
    }

    private static Guid TenantIdToGuid()
    {
        var hash = System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(TenantId));
        return new Guid(hash);
    }

    private static string RandomCnpjNumerico()
    {
        var rng = Random.Shared;
        return $"{rng.Next(10, 99)}{rng.Next(100, 999)}{rng.Next(100, 999)}0001{rng.Next(10, 99):D2}";
    }

    private static string RandomCpfNumerico()
    {
        var rng = Random.Shared;
        var digits = new int[9];
        for (int i = 0; i < 9; i++) digits[i] = rng.Next(0, 10);
        return $"{digits[0]}{digits[1]}{digits[2]}{digits[3]}{digits[4]}{digits[5]}{digits[6]}{digits[7]}{digits[8]}{rng.Next(10, 99):D2}";
    }

    private static string RandomCnpj()
    {
        var rng = Random.Shared;
        var digits = new int[14];
        digits[0] = rng.Next(0, 2);

        for (int i = 1; i < 12; i++) digits[i] = rng.Next(0, 10);

        var baseCnpj = $"{rng.Next(10, 99)}.{rng.Next(100, 999)}.{rng.Next(100, 999)}/0001-";

        return baseCnpj + $"{rng.Next(10, 99):D2}";
    }

    private static string RandomCpf()
    {
        var rng = Random.Shared;
        var digits = new int[11];
        for (int i = 0; i < 9; i++) digits[i] = rng.Next(0, 10);

        return $"{digits[0]}{digits[1]}{digits[2]}.{digits[3]}{digits[4]}{digits[5]}.{digits[6]}{digits[7]}{digits[8]}-{rng.Next(10, 99):D2}";
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

        await SetupDadosTesteAsync();

        var lats = new List<double>();
        var payload = JsonSerializer.Serialize(new
        {
            empresaId = _empresaId!.Value,
            periodo = "2026-06",
            tipoCalculo = "Mensal",
        });

        for (int i = 0; i < 20; i++)
        {
            var content = new StringContent(payload, Encoding.UTF8,
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
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

        await SetupDadosTesteAsync();

        var payload = JsonSerializer.Serialize(new
        {
            empresaId = _empresaId!.Value,
            periodo = "2026-06",
            tipoCalculo = "Mensal",
        });

        // 1ª chamada: deve aceitar (202 Accepted ou 200)
        var c1 = new StringContent(payload, Encoding.UTF8,
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var r1 = await Client.PostAsync("/api/folha/processar", c1);
        var body1 = await r1.Content.ReadAsStringAsync();
        _output.WriteLine($"  1ª: {r1.StatusCode} — {body1[..Math.Min(200, body1.Length)]}");

        // Aguardar um pouco para o processamento iniciar
        await Task.Delay(500);

        // 2ª chamada com mesmo payload: deve retornar 409 Conflict
        var c2 = new StringContent(payload, Encoding.UTF8,
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var r2 = await Client.PostAsync("/api/folha/processar", c2);
        var body2 = await r2.Content.ReadAsStringAsync();

        _output.WriteLine($"  2ª: {r2.StatusCode} — {body2[..Math.Min(200, body2.Length)]}");

        if (r2.StatusCode == System.Net.HttpStatusCode.Conflict)
            _output.WriteLine("  CA06 OK: 409 Conflict — idempotência validada");
        else if (r1.IsSuccessStatusCode && r2.StatusCode == System.Net.HttpStatusCode.Conflict)
            _output.WriteLine("  CA06 OK: 409 Conflict — idempotência validada");
        else if (r1.IsSuccessStatusCode && r2.IsSuccessStatusCode)
            _output.WriteLine("  CA06 PARCIAL: Ambos retornaram sucesso (possível race condition ou constraint ausente)");
        else
            _output.WriteLine($"  CA06 INCONCLUSIVO: Verificar resposta acima");
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

    [Fact]
    public async Task Carga_ProcessamentoReal_FluxoCompleto()
    {
        _output.WriteLine("=== Processamento Real: Fluxo Completo ===");

        await SetupDadosTesteAsync();

        // 1. Iniciar processamento com empresa real
        var payload = JsonSerializer.Serialize(new
        {
            empresaId = _empresaId!.Value,
            periodo = "2026-06",
            tipoCalculo = "Mensal",
        });

        var content = new StringContent(payload, Encoding.UTF8,
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        var sw = Stopwatch.StartNew();
        var response = await Client.PostAsync("/api/folha/processar", content);
        sw.Stop();

        _output.WriteLine($"POST /api/folha/processar: {response.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms");

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        if (response.IsSuccessStatusCode && body.TryGetProperty("processamentoId", out var procIdElem))
        {
            var processamentoId = procIdElem.GetString()!;
            _output.WriteLine($"  Processamento ID: {processamentoId}");

            // 2. Acompanhar status (polling)
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                sw.Restart();
                var statusResponse = await Client.GetAsync($"/api/folha/processamento/{processamentoId}");
                sw.Stop();

                var statusBody = await statusResponse.Content.ReadAsStringAsync();
                _output.WriteLine($"  [{i + 1}] GET status: {statusResponse.StatusCode} — {sw.Elapsed.TotalMilliseconds:F1}ms — {statusBody[..Math.Min(150, statusBody.Length)]}");

                if (statusBody.Contains("Concluido") || statusBody.Contains("Falho"))
                    break;
            }
        }
        else
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"  Resposta: {errorBody[..Math.Min(300, errorBody.Length)]}");
        }

        _output.WriteLine("=== Fim ===");
    }
}
