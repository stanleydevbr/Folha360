using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.ValueObjects;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class CriarCargoHandler : IRequestHandler<CriarCargoCommand, Result<CargoDto>>
{
    private readonly ICargoRepository _repo;
    public CriarCargoHandler(ICargoRepository repo) => _repo = repo;

    public async Task<Result<CargoDto>> Handle(CriarCargoCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<CargoDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");

        try
        {
            var cbo = new Cbo(cmd.Cbo);
            var cargo = new Cargo(cmd.EmpresaId, cmd.Nome, cbo.Codigo,
                cmd.Descricao, cmd.SalarioBaseMinimo, cmd.SalarioBaseMaximo);
            await _repo.AddAsync(cargo, ct);

            return Result<CargoDto>.Success(new CargoDto
            {
                Id = cargo.Id,
                EmpresaId = cargo.EmpresaId,
                Nome = cargo.Nome,
                Cbo = cargo.Cbo,
                Descricao = cargo.Descricao,
                SalarioBaseMinimo = cargo.SalarioBaseMinimo,
                SalarioBaseMaximo = cargo.SalarioBaseMaximo,
                CreatedAt = cargo.CreatedAt,
                UpdatedAt = cargo.UpdatedAt,
            });
        }
        catch (ArgumentException ex)
        {
            return Result<CargoDto>.Failure("VALIDACAO", ex.Message);
        }
    }
}

public class AtualizarCargoHandler : IRequestHandler<AtualizarCargoCommand, Result<CargoDto>>
{
    private readonly ICargoRepository _repo;
    public AtualizarCargoHandler(ICargoRepository repo) => _repo = repo;

    public async Task<Result<CargoDto>> Handle(AtualizarCargoCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<CargoDto>.Failure("VALIDACAO", "Id é obrigatório.");

        var cargo = await _repo.GetByIdAsync(cmd.Id, ct);
        if (cargo is null)
            return Result<CargoDto>.Failure("NAO_ENCONTRADO", "Cargo não encontrado.");

        try
        {
            var cbo = new Cbo(cmd.Cbo);
            cargo.Atualizar(cmd.Nome, cbo.Codigo, cmd.Descricao,
                cmd.SalarioBaseMinimo, cmd.SalarioBaseMaximo);
            await _repo.UpdateAsync(cargo, ct);

            return Result<CargoDto>.Success(new CargoDto
            {
                Id = cargo.Id,
                EmpresaId = cargo.EmpresaId,
                Nome = cargo.Nome,
                Cbo = cargo.Cbo,
                Descricao = cargo.Descricao,
                CreatedAt = cargo.CreatedAt,
                UpdatedAt = cargo.UpdatedAt,
            });
        }
        catch (ArgumentException ex)
        {
            return Result<CargoDto>.Failure("VALIDACAO", ex.Message);
        }
    }
}

public class ExcluirCargoHandler : IRequestHandler<ExcluirCargoCommand, Result<bool>>
{
    private readonly ICargoRepository _repo;
    public ExcluirCargoHandler(ICargoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirCargoCommand cmd, CancellationToken ct)
    {
        var temVinculos = await _repo.HasFuncionariosVinculadosAsync(cmd.Id, ct);
        if (temVinculos)
            return Result<bool>.Failure("VINCULO_ATIVO", "Cargo possui funcionários vinculados.");

        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterCargoHandler : IRequestHandler<ObterCargoQuery, Result<CargoDto>>
{
    private readonly ICargoRepository _repo;
    public ObterCargoHandler(ICargoRepository repo) => _repo = repo;

    public async Task<Result<CargoDto>> Handle(ObterCargoQuery query, CancellationToken ct)
    {
        var c = await _repo.GetByIdAsync(query.Id, ct);
        if (c is null)
            return Result<CargoDto>.Failure("NAO_ENCONTRADO", "Cargo não encontrado.");

        return Result<CargoDto>.Success(new CargoDto
        {
            Id = c.Id,
            EmpresaId = c.EmpresaId,
            Nome = c.Nome,
            Cbo = c.Cbo,
            Descricao = c.Descricao,
            SalarioBaseMinimo = c.SalarioBaseMinimo,
            SalarioBaseMaximo = c.SalarioBaseMaximo,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
        });
    }
}

public class ListarCargosHandler : IRequestHandler<ListarCargosQuery, PaginatedResult<CargoDto>>
{
    private readonly ICargoRepository _repo;
    public ListarCargosHandler(ICargoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<CargoDto>> Handle(ListarCargosQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy, query.EmpresaId, query.Nome, ct);

        var dtos = items.Select(c => new CargoDto
        {
            Id = c.Id,
            EmpresaId = c.EmpresaId,
            Nome = c.Nome,
            Cbo = c.Cbo,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
        });

        return PaginatedResult<CargoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
