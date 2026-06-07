using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using MediatR;

namespace Folha360.Eventos.Application.Handlers;

public class CriarAfastamentoHandler : IRequestHandler<CriarAfastamentoCommand, Result<AfastamentoDto>>
{
    private readonly IAfastamentoRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public CriarAfastamentoHandler(IAfastamentoRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<AfastamentoDto>> Handle(CriarAfastamentoCommand cmd, CancellationToken ct)
    {
        var entity = new Afastamento(
            cmd.FuncionarioId, cmd.EmpresaId, cmd.DataInicio, cmd.DataFimPrevista,
            cmd.TipoAfastamento, cmd.Cid);

        await _repo.AddAsync(entity, ct);
        await _publishEndpoint.Publish(new GerarXmlAfastamentoCommand(entity.Id), ct);

        return Result<AfastamentoDto>.Success(Map(entity));
    }

    private static AfastamentoDto Map(Afastamento e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataInicio = e.DataInicio,
        DataFimPrevista = e.DataFimPrevista,
        DataFimEfetiva = e.DataFimEfetiva,
        TipoAfastamento = e.TipoAfastamento,
        Cid = e.Cid,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarAfastamentoHandler : IRequestHandler<AtualizarAfastamentoCommand, Result<AfastamentoDto>>
{
    private readonly IAfastamentoRepository _repo;

    public AtualizarAfastamentoHandler(IAfastamentoRepository repo) => _repo = repo;

    public async Task<Result<AfastamentoDto>> Handle(AtualizarAfastamentoCommand cmd, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            return Result<AfastamentoDto>.Failure("NAO_ENCONTRADO", "Afastamento não encontrado.");

        entity.Atualizar(cmd.DataInicio, cmd.DataFimPrevista, cmd.TipoAfastamento, cmd.DataFimEfetiva, cmd.Cid);
        await _repo.UpdateAsync(entity, ct);

        return Result<AfastamentoDto>.Success(Map(entity));
    }

    private static AfastamentoDto Map(Afastamento e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataInicio = e.DataInicio,
        DataFimPrevista = e.DataFimPrevista,
        DataFimEfetiva = e.DataFimEfetiva,
        TipoAfastamento = e.TipoAfastamento,
        Cid = e.Cid,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class ExcluirAfastamentoHandler : IRequestHandler<ExcluirAfastamentoCommand, Result<bool>>
{
    private readonly IAfastamentoRepository _repo;
    public ExcluirAfastamentoHandler(IAfastamentoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirAfastamentoCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterAfastamentoHandler : IRequestHandler<ObterAfastamentoQuery, Result<AfastamentoDto>>
{
    private readonly IAfastamentoRepository _repo;
    public ObterAfastamentoHandler(IAfastamentoRepository repo) => _repo = repo;

    public async Task<Result<AfastamentoDto>> Handle(ObterAfastamentoQuery query, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return Result<AfastamentoDto>.Failure("NAO_ENCONTRADO", "Afastamento não encontrado.");

        return Result<AfastamentoDto>.Success(new AfastamentoDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataInicio = entity.DataInicio,
            DataFimPrevista = entity.DataFimPrevista,
            DataFimEfetiva = entity.DataFimEfetiva,
            TipoAfastamento = entity.TipoAfastamento,
            Cid = entity.Cid,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ListarAfastamentosHandler : IRequestHandler<ListarAfastamentosQuery, PaginatedResult<AfastamentoDto>>
{
    private readonly IAfastamentoRepository _repo;
    public ListarAfastamentosHandler(IAfastamentoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<AfastamentoDto>> Handle(ListarAfastamentosQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(query.Page, query.PageSize, query.FuncionarioId, ct);
        var dtos = items.Select(e => new AfastamentoDto
        {
            Id = e.Id,
            FuncionarioId = e.FuncionarioId,
            EmpresaId = e.EmpresaId,
            DataInicio = e.DataInicio,
            DataFimPrevista = e.DataFimPrevista,
            DataFimEfetiva = e.DataFimEfetiva,
            TipoAfastamento = e.TipoAfastamento,
            Cid = e.Cid,
            XmlContent = e.XmlContent,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<AfastamentoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
