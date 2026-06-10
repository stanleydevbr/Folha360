namespace Folha360.Relatorios.Application.Services;

public interface IRedisCacheService
{
    Task<T?> ObterAsync<T>(string chave, CancellationToken ct);
    Task ArmazenarAsync<T>(string chave, T valor, TimeSpan ttl, CancellationToken ct);
    Task InvalidarAsync(string chave, CancellationToken ct);
}
