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

public class CriarFeriasHandler : IRequestHandler<CriarFeriasCommand, Result<FeriasDto>>
{
    private readonly IFeriasRepository _repo;
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;

    public CriarFeriasHandler(IFeriasRepository repo, IMessageBus messageBus, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _messageBus = messageBus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<FeriasDto>> Handle(CriarFeriasCommand cmd, CancellationToken ct)
    {
        var entity = new Ferias(
            cmd.FuncionarioId, cmd.EmpresaId, cmd.DataInicio, cmd.DiasGozo,
            cmd.PeriodoAquisitivoInicio, cmd.PeriodoAquisitivoFim, cmd.TipoFerias);

        await _repo.AddAsync(entity, ct);

        await _messageBus.PublishAsync(
            new FeriasConcedidasEvent(entity.FuncionarioId, entity.EmpresaId, entity.Id),
            "folha360.eventos", "FeriasConcedidas", ct);

        await _publishEndpoint.Publish(new GerarXmlFeriasCommand(entity.Id), ct);

        return Result<FeriasDto>.Success(Map(entity));
    }

    private static FeriasDto Map(Ferias e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataInicio = e.DataInicio,
        DiasGozo = e.DiasGozo,
        PeriodoAquisitivoInicio = e.PeriodoAquisitivoInicio,
        PeriodoAquisitivoFim = e.PeriodoAquisitivoFim,
        TipoFerias = e.TipoFerias,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarFeriasHandler : IRequestHandler<AtualizarFeriasCommand, Result<FeriasDto>>
{
    private readonly IFeriasRepository _repo;

    public AtualizarFeriasHandler(IFeriasRepository repo) => _repo = repo;

    public async Task<Result<FeriasDto>> Handle(AtualizarFeriasCommand cmd, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            return Result<FeriasDto>.Failure("NAO_ENCONTRADO", "Férias não encontradas.");

        entity.Atualizar(cmd.DataInicio, cmd.DiasGozo, cmd.PeriodoAquisitivoInicio, cmd.PeriodoAquisitivoFim, cmd.TipoFerias);
        await _repo.UpdateAsync(entity, ct);

        return Result<FeriasDto>.Success(Map(entity));
    }

    private static FeriasDto Map(Ferias e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataInicio = e.DataInicio,
        DiasGozo = e.DiasGozo,
        PeriodoAquisitivoInicio = e.PeriodoAquisitivoInicio,
        PeriodoAquisitivoFim = e.PeriodoAquisitivoFim,
        TipoFerias = e.TipoFerias,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class ExcluirFeriasHandler : IRequestHandler<ExcluirFeriasCommand, Result<bool>>
{
    private readonly IFeriasRepository _repo;
    public ExcluirFeriasHandler(IFeriasRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirFeriasCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterFeriasHandler : IRequestHandler<ObterFeriasQuery, Result<FeriasDto>>
{
    private readonly IFeriasRepository _repo;
    public ObterFeriasHandler(IFeriasRepository repo) => _repo = repo;

    public async Task<Result<FeriasDto>> Handle(ObterFeriasQuery query, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return Result<FeriasDto>.Failure("NAO_ENCONTRADO", "Férias não encontradas.");

        return Result<FeriasDto>.Success(new FeriasDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataInicio = entity.DataInicio,
            DiasGozo = entity.DiasGozo,
            PeriodoAquisitivoInicio = entity.PeriodoAquisitivoInicio,
            PeriodoAquisitivoFim = entity.PeriodoAquisitivoFim,
            TipoFerias = entity.TipoFerias,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ListarFeriasHandler : IRequestHandler<ListarFeriasQuery, PaginatedResult<FeriasDto>>
{
    private readonly IFeriasRepository _repo;
    public ListarFeriasHandler(IFeriasRepository repo) => _repo = repo;

    public async Task<PaginatedResult<FeriasDto>> Handle(ListarFeriasQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(query.Page, query.PageSize, query.FuncionarioId, ct);
        var dtos = items.Select(e => new FeriasDto
        {
            Id = e.Id,
            FuncionarioId = e.FuncionarioId,
            EmpresaId = e.EmpresaId,
            DataInicio = e.DataInicio,
            DiasGozo = e.DiasGozo,
            PeriodoAquisitivoInicio = e.PeriodoAquisitivoInicio,
            PeriodoAquisitivoFim = e.PeriodoAquisitivoFim,
            TipoFerias = e.TipoFerias,
            XmlContent = e.XmlContent,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<FeriasDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
