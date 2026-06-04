namespace Folha360.Domain.Abstractions;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default)
        where T : class;

    Task ConsumeAsync<T>(
        string queue,
        string exchange,
        string routingKey,
        Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default)
        where T : class;
}
