using System.Text.Json;
using Folha360.Domain.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return Task.FromResult(value);
        }

        _logger.LogDebug("Cache miss for key: {Key}", key);
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(ttl)
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        _memoryCache.Set(key, value, options);
        _logger.LogDebug("Cache set for key: {Key} with TTL: {Ttl}", key, ttl);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        _logger.LogDebug("Cache removed for key: {Key}", key);

        return Task.CompletedTask;
    }
}
