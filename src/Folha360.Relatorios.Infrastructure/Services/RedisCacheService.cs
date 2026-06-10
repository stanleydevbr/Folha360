using System.Text.Json;
using Folha360.Relatorios.Application.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Folha360.Relatorios.Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisCacheService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> ObterAsync<T>(string chave, CancellationToken ct)
    {
        try
        {
            var db = _redis.GetDatabase();
            var valor = await db.StringGetAsync(chave);

            if (valor.IsNullOrEmpty)
            {
                _logger.LogDebug("Cache miss: {Chave}", chave);
                return default;
            }

            _logger.LogDebug("Cache hit: {Chave}", chave);
            var serialized = valor.ToString();
            return JsonSerializer.Deserialize<T>(serialized, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao obter cache Redis: {Chave}", chave);
            return default;
        }
    }

    public async Task ArmazenarAsync<T>(string chave, T valor, TimeSpan ttl, CancellationToken ct)
    {
        try
        {
            var db = _redis.GetDatabase();
            var serialized = JsonSerializer.Serialize(valor, JsonOptions);
            await db.StringSetAsync(chave, serialized, ttl);
            _logger.LogDebug("Cache armazenado: {Chave}, TTL: {TTL}", chave, ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao armazenar cache Redis: {Chave}", chave);
        }
    }

    public async Task InvalidarAsync(string chave, CancellationToken ct)
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(chave);
            _logger.LogDebug("Cache invalidado: {Chave}", chave);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao invalidar cache Redis: {Chave}", chave);
        }
    }
}
