using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Events;
using Folha360.Domain.Abstractions;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class CriarRubricaHandler : IRequestHandler<CriarRubricaCommand, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    private readonly IMessageBus _messageBus;

    public CriarRubricaHandler(IRubricaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaDto>> Handle(CriarRubricaCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<RubricaDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");

        var existente = await _repo.GetByCodigoAsync(cmd.EmpresaId, cmd.Codigo, ct);
        if (existente is not null)
            return Result<RubricaDto>.Failure("CODIGO_DUPLICADO", "Já existe uma rubrica com este código.");

        var rubrica = new Rubrica(cmd.EmpresaId, cmd.Codigo, cmd.Descricao, cmd.Natureza,
            cmd.TipoEsocial, cmd.IncideInss, cmd.IncideIrrf, cmd.IncideFgts,
            cmd.IncideContribuicaoSindical, cmd.IncideDecimoTerceiro,
            cmd.IncideFerias, cmd.IncideAvisoPrevio, cmd.FormulaCalculo, cmd.OrdemExibicao);

        await _repo.AddAsync(rubrica, ct);

        return Result<RubricaDto>.Success(MapRubrica(rubrica));
    }

    private static RubricaDto MapRubrica(Rubrica r) => new()
    {
        Id = r.Id,
        EmpresaId = r.EmpresaId,
        Codigo = r.Codigo,
        Descricao = r.Descricao,
        Natureza = r.Natureza,
        TipoEsocial = r.TipoEsocial,
        IncideInss = r.IncideInss,
        IncideIrrf = r.IncideIrrf,
        IncideFgts = r.IncideFgts,
        IncideContribuicaoSindical = r.IncideContribuicaoSindical,
        IncideDecimoTerceiro = r.IncideDecimoTerceiro,
        IncideFerias = r.IncideFerias,
        IncideAvisoPrevio = r.IncideAvisoPrevio,
        FormulaCalculo = r.FormulaCalculo,
        OrdemExibicao = r.OrdemExibicao,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };
}

public class AtualizarRubricaHandler : IRequestHandler<AtualizarRubricaCommand, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    private readonly IMessageBus _messageBus;

    public AtualizarRubricaHandler(IRubricaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaDto>> Handle(AtualizarRubricaCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<RubricaDto>.Failure("VALIDACAO", "Id é obrigatório.");

        var rubrica = await _repo.GetByIdAsync(cmd.Id, ct);
        if (rubrica is null)
            return Result<RubricaDto>.Failure("NAO_ENCONTRADO", "Rubrica não encontrada.");

        rubrica.Atualizar(cmd.Descricao, cmd.Natureza, cmd.TipoEsocial,
            cmd.IncideInss, cmd.IncideIrrf, cmd.IncideFgts, cmd.IncideContribuicaoSindical,
            cmd.IncideDecimoTerceiro, cmd.IncideFerias, cmd.IncideAvisoPrevio,
            cmd.FormulaCalculo, cmd.OrdemExibicao);

        await _repo.UpdateAsync(rubrica, ct);

        await _messageBus.PublishAsync(new RubricaAlteradaEvent(
            rubrica.Id, rubrica.EmpresaId, rubrica.Codigo, rubrica.Natureza,
            new RubricaIncidencias(rubrica.IncideInss, rubrica.IncideIrrf, rubrica.IncideFgts,
                rubrica.IncideContribuicaoSindical, rubrica.IncideDecimoTerceiro,
                rubrica.IncideFerias, rubrica.IncideAvisoPrevio)),
            "folha360.cadastros", "RubricaAlterada", ct);

        return Result<RubricaDto>.Success(new RubricaDto
        {
            Id = rubrica.Id,
            EmpresaId = rubrica.EmpresaId,
            Codigo = rubrica.Codigo,
            Descricao = rubrica.Descricao,
            Natureza = rubrica.Natureza,
            CreatedAt = rubrica.CreatedAt,
            UpdatedAt = rubrica.UpdatedAt,
        });
    }
}

public class ExcluirRubricaHandler : IRequestHandler<ExcluirRubricaCommand, Result<bool>>
{
    private readonly IRubricaRepository _repo;
    public ExcluirRubricaHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirRubricaCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterRubricaHandler : IRequestHandler<ObterRubricaQuery, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    public ObterRubricaHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<Result<RubricaDto>> Handle(ObterRubricaQuery query, CancellationToken ct)
    {
        var r = await _repo.GetByIdAsync(query.Id, ct);
        if (r is null)
            return Result<RubricaDto>.Failure("NAO_ENCONTRADO", "Rubrica não encontrada.");

        return Result<RubricaDto>.Success(new RubricaDto
        {
            Id = r.Id,
            EmpresaId = r.EmpresaId,
            Codigo = r.Codigo,
            Descricao = r.Descricao,
            Natureza = r.Natureza,
            TipoEsocial = r.TipoEsocial,
            IncideInss = r.IncideInss,
            IncideIrrf = r.IncideIrrf,
            IncideFgts = r.IncideFgts,
            IncideContribuicaoSindical = r.IncideContribuicaoSindical,
            IncideDecimoTerceiro = r.IncideDecimoTerceiro,
            IncideFerias = r.IncideFerias,
            IncideAvisoPrevio = r.IncideAvisoPrevio,
            FormulaCalculo = r.FormulaCalculo,
            OrdemExibicao = r.OrdemExibicao,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        });
    }
}

public class ListarRubricasHandler : IRequestHandler<ListarRubricasQuery, PaginatedResult<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    public ListarRubricasHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<PaginatedResult<RubricaDto>> Handle(ListarRubricasQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy,
            query.EmpresaId, query.Natureza, query.TipoEsocial, ct);

        var dtos = items.Select(r => new RubricaDto
        {
            Id = r.Id,
            EmpresaId = r.EmpresaId,
            Codigo = r.Codigo,
            Descricao = r.Descricao,
            Natureza = r.Natureza,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        });

        return PaginatedResult<RubricaDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
