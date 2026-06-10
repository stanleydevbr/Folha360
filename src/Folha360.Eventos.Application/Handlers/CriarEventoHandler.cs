using Folha360.Domain.Abstractions;
using MassTransit;
using MediatR;

namespace Folha360.Eventos.Application.Handlers;

/// <summary>
/// Handler base para operações de criação de eventos trabalhistas.
/// Encapsula o fluxo comum: criar entidade → salvar → publicar domain event → publicar comando XML → retornar DTO.
/// </summary>
/// <typeparam name="TCommand">Tipo do comando MediatR.</typeparam>
/// <typeparam name="TEntity">Tipo da entidade de domínio.</typeparam>
/// <typeparam name="TDto">Tipo do DTO de resposta.</typeparam>
public abstract class CriarEventoHandler<TCommand, TEntity, TDto>
    : IRequestHandler<TCommand, Result<TDto>>
    where TCommand : IRequest<Result<TDto>>
{
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;

    protected CriarEventoHandler(IMessageBus messageBus, IPublishEndpoint publishEndpoint)
    {
        _messageBus = messageBus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<TDto>> Handle(TCommand cmd, CancellationToken ct)
    {
        var entity = CriarEntidade(cmd);
        await SalvarEntidadeAsync(entity, ct);
        await PublicarEventosAsync(entity, ct);
        return Result<TDto>.Success(MapParaDto(entity));
    }

    /// <summary>
    /// Cria a entidade de domínio a partir do comando.
    /// </summary>
    protected abstract TEntity CriarEntidade(TCommand cmd);

    /// <summary>
    /// Salva a entidade no repositório.
    /// </summary>
    protected abstract Task SalvarEntidadeAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Publica os eventos de domínio e comandos de geração de XML.
    /// </summary>
    protected abstract Task PublicarEventosAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Mapeia a entidade para o DTO de resposta.
    /// </summary>
    protected abstract TDto MapParaDto(TEntity entity);

    /// <summary>
    /// Publica um domain event via IMessageBus.
    /// </summary>
    protected async Task PublicarDomainEventAsync<TEvent>(TEvent domainEvent, string routingKey, CancellationToken ct)
        where TEvent : class
    {
        await _messageBus.PublishAsync(domainEvent, "folha360.eventos", routingKey, ct);
    }

    /// <summary>
    /// Publica um comando interno de geração de XML via IPublishEndpoint.
    /// </summary>
    protected async Task PublicarComandoXmlAsync<TXmlCommand>(TXmlCommand comando, CancellationToken ct)
        where TXmlCommand : class
    {
        await _publishEndpoint.Publish(comando, ct);
    }
}
