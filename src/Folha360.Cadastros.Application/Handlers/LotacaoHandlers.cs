using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class CriarLotacaoHandler : IRequestHandler<CriarLotacaoCommand, Result<LotacaoDto>>
{
    private readonly ILotacaoRepository _repo;
    public CriarLotacaoHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<Result<LotacaoDto>> Handle(CriarLotacaoCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<LotacaoDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");

        var existente = await _repo.GetByCodigoAsync(cmd.EmpresaId, cmd.Codigo, ct);
        if (existente is not null)
            return Result<LotacaoDto>.Failure("CODIGO_DUPLICADO", "Já existe uma lotação com este código.");

        var lotacao = new Lotacao(cmd.EmpresaId, cmd.Codigo, cmd.Descricao, cmd.TipoEsocial);
        await _repo.AddAsync(lotacao, ct);

        return Result<LotacaoDto>.Success(new LotacaoDto
        {
            Id = lotacao.Id,
            EmpresaId = lotacao.EmpresaId,
            Codigo = lotacao.Codigo,
            Descricao = lotacao.Descricao,
            TipoEsocial = lotacao.TipoEsocial,
            CreatedAt = lotacao.CreatedAt,
            UpdatedAt = lotacao.UpdatedAt,
        });
    }
}

public class AtualizarLotacaoHandler : IRequestHandler<AtualizarLotacaoCommand, Result<LotacaoDto>>
{
    private readonly ILotacaoRepository _repo;
    public AtualizarLotacaoHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<Result<LotacaoDto>> Handle(AtualizarLotacaoCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<LotacaoDto>.Failure("VALIDACAO", "Id é obrigatório.");

        var lotacao = await _repo.GetByIdAsync(cmd.Id, ct);
        if (lotacao is null)
            return Result<LotacaoDto>.Failure("NAO_ENCONTRADO", "Lotação não encontrada.");

        lotacao.Atualizar(cmd.Codigo, cmd.Descricao, cmd.TipoEsocial);
        await _repo.UpdateAsync(lotacao, ct);

        return Result<LotacaoDto>.Success(new LotacaoDto
        {
            Id = lotacao.Id,
            EmpresaId = lotacao.EmpresaId,
            Codigo = lotacao.Codigo,
            Descricao = lotacao.Descricao,
            TipoEsocial = lotacao.TipoEsocial,
            CreatedAt = lotacao.CreatedAt,
            UpdatedAt = lotacao.UpdatedAt,
        });
    }
}

public class ExcluirLotacaoHandler : IRequestHandler<ExcluirLotacaoCommand, Result<bool>>
{
    private readonly ILotacaoRepository _repo;
    public ExcluirLotacaoHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirLotacaoCommand cmd, CancellationToken ct)
    {
        var temVinculos = await _repo.HasFuncionariosVinculadosAsync(cmd.Id, ct);
        if (temVinculos)
            return Result<bool>.Failure("VINCULO_ATIVO", "Lotação possui funcionários vinculados.");

        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterLotacaoHandler : IRequestHandler<ObterLotacaoQuery, Result<LotacaoDto>>
{
    private readonly ILotacaoRepository _repo;
    public ObterLotacaoHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<Result<LotacaoDto>> Handle(ObterLotacaoQuery query, CancellationToken ct)
    {
        var l = await _repo.GetByIdAsync(query.Id, ct);
        if (l is null)
            return Result<LotacaoDto>.Failure("NAO_ENCONTRADO", "Lotação não encontrada.");

        return Result<LotacaoDto>.Success(new LotacaoDto
        {
            Id = l.Id,
            EmpresaId = l.EmpresaId,
            Codigo = l.Codigo,
            Descricao = l.Descricao,
            TipoEsocial = l.TipoEsocial,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        });
    }
}

public class ListarLotacoesHandler : IRequestHandler<ListarLotacoesQuery, PaginatedResult<LotacaoDto>>
{
    private readonly ILotacaoRepository _repo;
    public ListarLotacoesHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<LotacaoDto>> Handle(ListarLotacoesQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy, query.EmpresaId, query.Codigo, ct);

        var dtos = items.Select(l => new LotacaoDto
        {
            Id = l.Id,
            EmpresaId = l.EmpresaId,
            Codigo = l.Codigo,
            Descricao = l.Descricao,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
        });

        return PaginatedResult<LotacaoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
