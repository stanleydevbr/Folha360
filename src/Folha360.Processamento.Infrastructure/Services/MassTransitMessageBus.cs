using Folha360.Domain.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Infrastructure.Services;

public class MassTransitMessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitMessageBus> _logger;

    public MassTransitMessageBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitMessageBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken ct = default)
        where T : class
    {
        _logger.LogInformation(
            "Publishing message {MessageType} to exchange {Exchange}, routingKey: {RoutingKey}",
            typeof(T).Name, exchange, routingKey);

        await _publishEndpoint.Publish(message, ct);
    }

    public Task ConsumeAsync<T>(
        string queue, string exchange, string routingKey,
        Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default)
        where T : class
    {
        _logger.LogInformation(
            "Consume registration for queue {Queue}, exchange {Exchange}, routingKey {RoutingKey}",
            queue, exchange, routingKey);

        return Task.CompletedTask;
    }
}
