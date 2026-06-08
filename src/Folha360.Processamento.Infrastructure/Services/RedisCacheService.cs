using System.Text.Json;
using Folha360.Processamento.Application.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Folha360.Processamento.Infrastructure.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer? redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (_redis is null)
        {
            _logger.LogWarning("Redis indisponível — cache miss para chave {Key}", key);
            return null;
        }

        try
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<T>((string)value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao ler cache Redis para chave {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        if (_redis is null)
            return;

        try
        {
            var db = _redis.GetDatabase();
            var serialized = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serialized, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao escrever cache Redis para chave {Key}", key);
        }
    }

    public async Task InvalidateAsync(string pattern, CancellationToken ct = default)
    {
        if (_redis is null)
            return;

        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();

            if (keys.Length > 0)
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(keys);
                _logger.LogInformation("Cache invalidado: {Count} chaves com padrão {Pattern}", keys.Length, pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao invalidar cache Redis com padrão {Pattern}", pattern);
        }
    }

    public async Task InvalidateRubricasAsync(Guid empresaId, CancellationToken ct = default)
    {
        await InvalidateAsync($"cache:rubricas:{empresaId}*", ct);
    }

    public async Task SubscribeToInvalidationAsync(Func<string, Task> handler, CancellationToken ct = default)
    {
        if (_redis is null)
            return;

        try
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync(new RedisChannel("invalidate:rubricas", RedisChannel.PatternMode.Auto), (channel, message) =>
            {
                _ = handler(message!);
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao assinar canal de invalidação Redis");
        }
    }
}
