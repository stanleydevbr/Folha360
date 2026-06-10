using Folha360.Processamento.Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Consumers;

public class RubricaAlteradaConsumer : IConsumer<Folha360.Cadastros.Domain.Events.RubricaAlteradaEvent>
{
    private readonly IRedisCacheService _cache;
    private readonly ILogger<RubricaAlteradaConsumer> _logger;

    public RubricaAlteradaConsumer(IRedisCacheService cache, ILogger<RubricaAlteradaConsumer> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Folha360.Cadastros.Domain.Events.RubricaAlteradaEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Invalidando cache de rubricas para empresa {EmpresaId}", msg.EmpresaId);
        await _cache.InvalidateRubricasAsync(msg.EmpresaId, context.CancellationToken);
    }
}
