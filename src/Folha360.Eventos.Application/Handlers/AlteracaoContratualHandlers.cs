using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using Folha360.Eventos.Domain.Abstractions;
using Folha360.Eventos.Domain.Entities;
using Folha360.Eventos.Domain.Events;
using MassTransit;
using MediatR;

namespace Folha360.Eventos.Application.Handlers;

public class CriarAlteracaoContratualHandler : IRequestHandler<CriarAlteracaoContratualCommand, Result<AlteracaoContratualDto>>
{
    private readonly IAlteracaoContratualRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public CriarAlteracaoContratualHandler(IAlteracaoContratualRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<AlteracaoContratualDto>> Handle(CriarAlteracaoContratualCommand cmd, CancellationToken ct)
    {
        var entity = new AlteracaoContratual(
            cmd.FuncionarioId, cmd.EmpresaId, cmd.DataAlteracao,
            cmd.CamposAlterados, cmd.ValorAnterior, cmd.ValorNovo);

        await _repo.AddAsync(entity, ct);
        await _publishEndpoint.Publish(new GerarXmlAlteracaoContratualCommand(entity.Id), ct);

        return Result<AlteracaoContratualDto>.Success(Map(entity));
    }

    private static AlteracaoContratualDto Map(AlteracaoContratual e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataAlteracao = e.DataAlteracao,
        CamposAlterados = e.CamposAlterados,
        ValorAnterior = e.ValorAnterior,
        ValorNovo = e.ValorNovo,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarAlteracaoContratualHandler : IRequestHandler<AtualizarAlteracaoContratualCommand, Result<AlteracaoContratualDto>>
{
    private readonly IAlteracaoContratualRepository _repo;

    public AtualizarAlteracaoContratualHandler(IAlteracaoContratualRepository repo) => _repo = repo;

    public async Task<Result<AlteracaoContratualDto>> Handle(AtualizarAlteracaoContratualCommand cmd, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            return Result<AlteracaoContratualDto>.Failure("NAO_ENCONTRADO", "Alteração contratual não encontrada.");

        entity.Atualizar(cmd.DataAlteracao, cmd.CamposAlterados, cmd.ValorAnterior, cmd.ValorNovo);
        await _repo.UpdateAsync(entity, ct);

        return Result<AlteracaoContratualDto>.Success(Map(entity));
    }

    private static AlteracaoContratualDto Map(AlteracaoContratual e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataAlteracao = e.DataAlteracao,
        CamposAlterados = e.CamposAlterados,
        ValorAnterior = e.ValorAnterior,
        ValorNovo = e.ValorNovo,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class ExcluirAlteracaoContratualHandler : IRequestHandler<ExcluirAlteracaoContratualCommand, Result<bool>>
{
    private readonly IAlteracaoContratualRepository _repo;
    public ExcluirAlteracaoContratualHandler(IAlteracaoContratualRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirAlteracaoContratualCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterAlteracaoContratualHandler : IRequestHandler<ObterAlteracaoContratualQuery, Result<AlteracaoContratualDto>>
{
    private readonly IAlteracaoContratualRepository _repo;
    public ObterAlteracaoContratualHandler(IAlteracaoContratualRepository repo) => _repo = repo;

    public async Task<Result<AlteracaoContratualDto>> Handle(ObterAlteracaoContratualQuery query, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return Result<AlteracaoContratualDto>.Failure("NAO_ENCONTRADO", "Alteração contratual não encontrada.");

        return Result<AlteracaoContratualDto>.Success(new AlteracaoContratualDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataAlteracao = entity.DataAlteracao,
            CamposAlterados = entity.CamposAlterados,
            ValorAnterior = entity.ValorAnterior,
            ValorNovo = entity.ValorNovo,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ListarAlteracoesContratuaisHandler : IRequestHandler<ListarAlteracoesContratuaisQuery, PaginatedResult<AlteracaoContratualDto>>
{
    private readonly IAlteracaoContratualRepository _repo;
    public ListarAlteracoesContratuaisHandler(IAlteracaoContratualRepository repo) => _repo = repo;

    public async Task<PaginatedResult<AlteracaoContratualDto>> Handle(ListarAlteracoesContratuaisQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(query.Page, query.PageSize, query.FuncionarioId, ct);
        var dtos = items.Select(e => new AlteracaoContratualDto
        {
            Id = e.Id,
            FuncionarioId = e.FuncionarioId,
            EmpresaId = e.EmpresaId,
            DataAlteracao = e.DataAlteracao,
            CamposAlterados = e.CamposAlterados,
            ValorAnterior = e.ValorAnterior,
            ValorNovo = e.ValorNovo,
            XmlContent = e.XmlContent,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<AlteracaoContratualDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
