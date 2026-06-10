namespace Folha360.Fiscais.Application.Services;

/// <summary>
/// Cache Redis para regras fiscais e dados frequentemente acessados.
/// </summary>
public interface IRedisCacheService
{
    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Armazena um valor no cache com TTL opcional.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class;

    /// <summary>
    /// Invalida chaves que correspondem ao padrão.
    /// </summary>
    Task InvalidateAsync(string pattern, CancellationToken ct = default);
}
