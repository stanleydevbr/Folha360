using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Folha360.Cadastros.Application.Handlers;

/// <summary>
/// Handlers for shared domain table queries (lookups).
/// Uses IMemoryCache with 24h TTL since data changes very rarely.
/// </summary>
public class ListarCbosHandler : IRequestHandler<ListarCbosQuery, Result<List<CboDto>>>
{
    private readonly ICboRepository _repo;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public ListarCbosHandler(ICboRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<List<CboDto>>> Handle(ListarCbosQuery query, CancellationToken ct)
    {
        var cacheKey = $"lookup:cbos:{query.Filtro ?? "*"}:{query.ApenasAtivos}";
        if (_cache.TryGetValue(cacheKey, out List<CboDto>? cached) && cached is not null)
            return Result<List<CboDto>>.Success(cached);

        var items = await _repo.ListarAsync(query.Filtro, query.ApenasAtivos, ct);
        var dtos = items.Select(c => new CboDto
        {
            Id = c.Id,
            Codigo = c.Codigo,
            Titulo = c.Titulo,
            Ativo = c.Ativo
        }).ToList();

        _cache.Set(cacheKey, dtos, CacheTtl);
        return Result<List<CboDto>>.Success(dtos);
    }
}

public class ListarNaturezasJuridicasHandler : IRequestHandler<ListarNaturezasJuridicasQuery, Result<List<NaturezaJuridicaDto>>>
{
    private readonly INaturezaJuridicaRepository _repo;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public ListarNaturezasJuridicasHandler(INaturezaJuridicaRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<List<NaturezaJuridicaDto>>> Handle(ListarNaturezasJuridicasQuery query, CancellationToken ct)
    {
        var cacheKey = $"lookup:naturezas_juridicas:{query.ApenasAtivos}";
        if (_cache.TryGetValue(cacheKey, out List<NaturezaJuridicaDto>? cached) && cached is not null)
            return Result<List<NaturezaJuridicaDto>>.Success(cached);

        var items = await _repo.ListarAsync(query.ApenasAtivos, ct);
        var dtos = items.Select(n => new NaturezaJuridicaDto
        {
            Id = n.Id,
            Codigo = n.Codigo,
            Descricao = n.Descricao,
            Ativo = n.Ativo
        }).ToList();

        _cache.Set(cacheKey, dtos, CacheTtl);
        return Result<List<NaturezaJuridicaDto>>.Success(dtos);
    }
}

public class ListarMunicipiosHandler : IRequestHandler<ListarMunicipiosQuery, PaginatedResult<MunicipioIBGEDto>>
{
    private readonly IMunicipioIBGERepository _repo;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public ListarMunicipiosHandler(IMunicipioIBGERepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<PaginatedResult<MunicipioIBGEDto>> Handle(ListarMunicipiosQuery query, CancellationToken ct)
    {
        var cacheKey = $"lookup:municipios:{query.Uf}:{query.Nome}:{query.Page}:{query.PageSize}:{query.ApenasAtivos}";
        if (_cache.TryGetValue(cacheKey, out PaginatedResult<MunicipioIBGEDto>? cached) && cached is not null)
            return cached;

        var (items, total) = await _repo.ListarAsync(query.Uf, query.Nome, query.Page, query.PageSize, query.ApenasAtivos, ct);
        var dtos = items.Select(m => new MunicipioIBGEDto
        {
            Id = m.Id,
            CodigoIbge = m.CodigoIbge,
            Nome = m.Nome,
            Uf = m.Uf,
            CodigoUf = m.CodigoUf,
            Ativo = m.Ativo
        }).ToList();

        var result = PaginatedResult<MunicipioIBGEDto>.Success(dtos, query.Page, query.PageSize, total);

        _cache.Set(cacheKey, result, CacheTtl);
        return result;
    }
}

public class ListarBancosHandler : IRequestHandler<ListarBancosQuery, Result<List<BancoFebrabanDto>>>
{
    private readonly IBancoFebrabanRepository _repo;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public ListarBancosHandler(IBancoFebrabanRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Result<List<BancoFebrabanDto>>> Handle(ListarBancosQuery query, CancellationToken ct)
    {
        var cacheKey = $"lookup:bancos:{query.Filtro ?? "*"}:{query.ApenasAtivos}";
        if (_cache.TryGetValue(cacheKey, out List<BancoFebrabanDto>? cached) && cached is not null)
            return Result<List<BancoFebrabanDto>>.Success(cached);

        var items = await _repo.ListarAsync(query.Filtro, query.ApenasAtivos, ct);
        var dtos = items.Select(b => new BancoFebrabanDto
        {
            Id = b.Id,
            Codigo = b.Codigo,
            Nome = b.Nome,
            Ativo = b.Ativo
        }).ToList();

        _cache.Set(cacheKey, dtos, CacheTtl);
        return Result<List<BancoFebrabanDto>>.Success(dtos);
    }
}
