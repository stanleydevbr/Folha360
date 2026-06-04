using System.Text.Json;
using Folha360.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Services;

public class MessageBus : IMessageBus
{
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(ILogger<MessageBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default)
        where T : class
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation(
            "Message published to exchange: {Exchange}, routingKey: {RoutingKey}, message: {Message}",
            exchange, routingKey, messageJson);

        // TODO: Implementar RabbitMQ publisher na F03
        return Task.CompletedTask;
    }

    public Task ConsumeAsync<T>(
        string queue,
        string exchange,
        string routingKey,
        Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default)
        where T : class
    {
        _logger.LogInformation(
            "Consumer registered for queue: {Queue}, exchange: {Exchange}, routingKey: {RoutingKey}",
            queue, exchange, routingKey);

        // TODO: Implementar RabbitMQ consumer na F03
        return Task.CompletedTask;
    }
}
