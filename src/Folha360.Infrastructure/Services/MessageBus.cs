using System.Text.Json;
using Folha360.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Services;

public class MessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(IPublishEndpoint publishEndpoint, ILogger<MessageBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default)
        where T : class
    {
        var messageJson = JsonSerializer.Serialize(message);
        _logger.LogInformation(
            "Publishing message {MessageType} to exchange {Exchange}, routingKey: {RoutingKey}, message: {Message}",
            typeof(T).Name, exchange, routingKey, messageJson);

        await _publishEndpoint.Publish(message, ct);
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
            "Consumer registration for queue {Queue}, exchange {Exchange}, routingKey {RoutingKey} — MassTransit consumers are registered declaratively via AddConsumer<T>() in DI",
            queue, exchange, routingKey);

        // MassTransit consumers are registered declaratively via AddConsumer<T>() in DI.
        // The ConsumeAsync method on IMessageBus exists for interface compatibility;
        // actual message consumption is handled by MassTransit's IConsumer<T> implementations.
        return Task.CompletedTask;
    }
}
