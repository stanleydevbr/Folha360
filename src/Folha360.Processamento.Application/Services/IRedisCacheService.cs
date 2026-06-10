namespace Folha360.Processamento.Application.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default) where T : class;
    Task InvalidateAsync(string pattern, CancellationToken ct = default);
    Task InvalidateRubricasAsync(Guid empresaId, CancellationToken ct = default);
    Task SubscribeToInvalidationAsync(Func<string, Task> handler, CancellationToken ct = default);
}
