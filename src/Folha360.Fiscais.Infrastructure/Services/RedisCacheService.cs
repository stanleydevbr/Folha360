using System.Collections.Concurrent;
using System.Text.Json;
using Folha360.Fiscais.Application.Services;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Infrastructure.Services;

/// <summary>
/// Stub do cache Redis. Usa dicionário em memória como fallback.
/// Será substituído por StackExchange.Redis em produção.
/// </summary>
public class RedisCacheService : IRedisCacheService
{
    private readonly ConcurrentDictionary<string, (string Value, DateTime? ExpiresAt)> _cache = new();
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(ILogger<RedisCacheService> logger)
    {
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt.HasValue && entry.ExpiresAt.Value < DateTime.UtcNow)
            {
                _cache.TryRemove(key, out _);
                _logger.LogDebug("[STUB Redis] Cache miss (expired): {Key}", key);
                return Task.FromResult<T?>(null);
            }

            _logger.LogDebug("[STUB Redis] Cache hit: {Key}", key);
            var value = JsonSerializer.Deserialize<T>(entry.Value);
            return Task.FromResult(value);
        }

        _logger.LogDebug("[STUB Redis] Cache miss: {Key}", key);
        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        var expiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : (DateTime?)null;
        _cache[key] = (json, expiresAt);
        _logger.LogDebug("[STUB Redis] Cache set: {Key} (TTL: {Ttl})", key, ttl);
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string pattern, CancellationToken ct = default)
    {
        var keysToRemove = _cache.Keys
            .Where(k => k.Contains(pattern.Replace("*", ""), StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }

        _logger.LogDebug("[STUB Redis] Invalidated {Count} keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
        return Task.CompletedTask;
    }
}
