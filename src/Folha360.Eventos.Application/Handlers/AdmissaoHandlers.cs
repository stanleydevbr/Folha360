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

public class CriarAdmissaoHandler : IRequestHandler<CriarAdmissaoCommand, Result<AdmissaoDto>>
{
    private readonly IAdmissaoRepository _repo;
    private readonly IMessageBus _messageBus;
    private readonly IPublishEndpoint _publishEndpoint;

    public CriarAdmissaoHandler(IAdmissaoRepository repo, IMessageBus messageBus, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _messageBus = messageBus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<AdmissaoDto>> Handle(CriarAdmissaoCommand cmd, CancellationToken ct)
    {
        var entity = new Admissao(
            cmd.FuncionarioId, cmd.EmpresaId, cmd.DataAdmissao,
            cmd.CargoId, cmd.SalarioInicial, cmd.TipoContrato, cmd.PeriodoExperienciaMeses);

        await _repo.AddAsync(entity, ct);

        await _messageBus.PublishAsync(
            new FuncionarioAdmitidoEvent(entity.FuncionarioId, entity.EmpresaId, entity.Id),
            "folha360.eventos", "FuncionarioAdmitido", ct);

        await _publishEndpoint.Publish(new GerarXmlAdmissaoCommand(entity.Id), ct);

        return Result<AdmissaoDto>.Success(Map(entity));
    }

    private static AdmissaoDto Map(Admissao e) => new()
    {
        Id = e.Id,
        FuncionarioId = e.FuncionarioId,
        EmpresaId = e.EmpresaId,
        DataAdmissao = e.DataAdmissao,
        CargoId = e.CargoId,
        SalarioInicial = e.SalarioInicial,
        TipoContrato = e.TipoContrato,
        PeriodoExperienciaMeses = e.PeriodoExperienciaMeses,
        XmlContent = e.XmlContent,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarAdmissaoHandler : IRequestHandler<AtualizarAdmissaoCommand, Result<AdmissaoDto>>
{
    private readonly IAdmissaoRepository _repo;

    public AtualizarAdmissaoHandler(IAdmissaoRepository repo) => _repo = repo;

    public async Task<Result<AdmissaoDto>> Handle(AtualizarAdmissaoCommand cmd, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            return Result<AdmissaoDto>.Failure("NAO_ENCONTRADO", "Admissão não encontrada.");

        entity.Atualizar(cmd.DataAdmissao, cmd.CargoId, cmd.SalarioInicial, cmd.TipoContrato, cmd.PeriodoExperienciaMeses);
        await _repo.UpdateAsync(entity, ct);

        return Result<AdmissaoDto>.Success(new AdmissaoDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataAdmissao = entity.DataAdmissao,
            CargoId = entity.CargoId,
            SalarioInicial = entity.SalarioInicial,
            TipoContrato = entity.TipoContrato,
            PeriodoExperienciaMeses = entity.PeriodoExperienciaMeses,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ExcluirAdmissaoHandler : IRequestHandler<ExcluirAdmissaoCommand, Result<bool>>
{
    private readonly IAdmissaoRepository _repo;

    public ExcluirAdmissaoHandler(IAdmissaoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirAdmissaoCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterAdmissaoHandler : IRequestHandler<ObterAdmissaoQuery, Result<AdmissaoDto>>
{
    private readonly IAdmissaoRepository _repo;

    public ObterAdmissaoHandler(IAdmissaoRepository repo) => _repo = repo;

    public async Task<Result<AdmissaoDto>> Handle(ObterAdmissaoQuery query, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(query.Id, ct);
        if (entity is null)
            return Result<AdmissaoDto>.Failure("NAO_ENCONTRADO", "Admissão não encontrada.");

        return Result<AdmissaoDto>.Success(new AdmissaoDto
        {
            Id = entity.Id,
            FuncionarioId = entity.FuncionarioId,
            EmpresaId = entity.EmpresaId,
            DataAdmissao = entity.DataAdmissao,
            CargoId = entity.CargoId,
            SalarioInicial = entity.SalarioInicial,
            TipoContrato = entity.TipoContrato,
            PeriodoExperienciaMeses = entity.PeriodoExperienciaMeses,
            XmlContent = entity.XmlContent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        });
    }
}

public class ListarAdmissoesHandler : IRequestHandler<ListarAdmissoesQuery, PaginatedResult<AdmissaoDto>>
{
    private readonly IAdmissaoRepository _repo;

    public ListarAdmissoesHandler(IAdmissaoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<AdmissaoDto>> Handle(ListarAdmissoesQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(query.Page, query.PageSize, query.FuncionarioId, ct);
        var dtos = items.Select(e => new AdmissaoDto
        {
            Id = e.Id,
            FuncionarioId = e.FuncionarioId,
            EmpresaId = e.EmpresaId,
            DataAdmissao = e.DataAdmissao,
            CargoId = e.CargoId,
            SalarioInicial = e.SalarioInicial,
            TipoContrato = e.TipoContrato,
            PeriodoExperienciaMeses = e.PeriodoExperienciaMeses,
            XmlContent = e.XmlContent,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<AdmissaoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
