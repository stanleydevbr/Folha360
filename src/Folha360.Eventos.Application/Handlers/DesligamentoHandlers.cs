using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Domain.Events;
using Folha360.Domain.Abstractions;
using MassTransit;
using MediatR;

namespace Folha360.Eventos.Application.Handlers;

public class CriarDesligamentoHandler : IRequestHandler<CriarDesligamentoCommand, Result<DesligamentoDto>>
{
    private readonly IDesligamentoRepository _repo;
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;

    public CriarDesligamentoHandler(IDesligamentoRepository repo, IMessageBus messageBus, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _messageBus = messageBus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<DesligamentoDto>> Handle(CriarDesligamentoCommand cmd, CancellationToken ct)
    {
        var entity = new Desligamento(
            cmd.FuncionarioId, cmd.EmpresaId, cmd.DataDesligamento,
            cmd.MotivoDesligamento, cmd.VerbasRescisorias);

        await _repo.AddAsync(entity, ct);

        await _messageBus.PublishAsync(
            new FuncionarioDesligadoEvent(entity.FuncionarioId, entity.EmpresaId, entity.Id),
            "folha360.eventos", "FuncionarioDesligado", ct);

        await _publishEndpoint.Publish(new GerarXmlDesligamentoCommand(entity.Id), ct);

        return Result<DesligamentoDto>.Success(Map(entity));
    }

    private static DesligamentoDto Map(Desligamento e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataDesligamento = e.DataDesligamento,
        MotivoDesligamento = e.MotivoDesligamento,
        VerbasRescisorias = e.VerbasRescisorias,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarDesligamentoHandler : IRequestHandler<AtualizarDesligamentoCommand, Result<DesligamentoDto>>
{
    private readonly IDesligamentoRepository _repo;

    public AtualizarDesligamentoHandler(IDesligamentoRepository repo) => _repo = repo;

    public async Task<Result<DesligamentoDto>> Handle(AtualizarDesligamentoCommand cmd, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            return Result<DesligamentoDto>.Failure("NAO_ENCONTRADO", "Desligamento não encontrado.");

        entity.Atualizar(cmd.DataDesligamento, cmd.MotivoDesligamento, cmd.VerbasRescisorias);
        await _repo.UpdateAsync(entity, ct);

        return Result<DesligamentoDto>.Success(Map(entity));
    }

    private static DesligamentoDto Map(Desligamento e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataDesligamento = e.DataDesligamento,
        MotivoDesligamento = e.MotivoDesligamento,
        VerbasRescisorias = e.VerbasRescisorias,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class ExcluirDesligamentoHandler : IRequestHandler<ExcluirDesligamentoCommand, Result<bool>>
{
    private readonly IDesligamentoRepository _repo;
    public ExcluirDesligamentoHandler(IDesligamentoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirDesligamentoCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterDesligamentoHandler : IRequestHandler<ObterDesligamentoQuery, Result<DesligamentoDto>>
{
    private readonly IDesligamentoRepository _repo;
    public ObterDesligamentoHandler(IDesligamentoRepository repo) => _repo = repo;

    public async Task<Result<DesligamentoDto>> Handle(ObterDesligamentoQuery query, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return Result<DesligamentoDto>.Failure("NAO_ENCONTRADO", "Desligamento não encontrado.");

        return Result<DesligamentoDto>.Success(new DesligamentoDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataDesligamento = entity.DataDesligamento,
            MotivoDesligamento = entity.MotivoDesligamento,
            VerbasRescisorias = entity.VerbasRescisorias,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ListarDesligamentosHandler : IRequestHandler<ListarDesligamentosQuery, PaginatedResult<DesligamentoDto>>
{
    private readonly IDesligamentoRepository _repo;
    public ListarDesligamentosHandler(IDesligamentoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<DesligamentoDto>> Handle(ListarDesligamentosQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(query.Page, query.PageSize, query.FuncionarioId, ct);
        var dtos = items.Select(e => new DesligamentoDto
        {
            Id = e.Id,
            FuncionarioId = e.FuncionarioId,
            EmpresaId = e.EmpresaId,
            DataDesligamento = e.DataDesligamento,
            MotivoDesligamento = e.MotivoDesligamento,
            VerbasRescisorias = e.VerbasRescisorias,
            XmlContent = e.XmlContent,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<DesligamentoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
