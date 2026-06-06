using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Events;
using Folha360.Cadastros.Domain.ValueObjects;
using Folha360.Domain.Abstractions;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class CriarFuncionarioHandler : IRequestHandler<CriarFuncionarioCommand, Result<FuncionarioDto>>
{
    private readonly IFuncionarioRepository _repo;
    private readonly IMessageBus _messageBus;

    public CriarFuncionarioHandler(IFuncionarioRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<FuncionarioDto>> Handle(CriarFuncionarioCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<FuncionarioDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");
        if (cmd.CargoId == Guid.Empty)
            return Result<FuncionarioDto>.Failure("VALIDACAO", "CargoId é obrigatório.");
        if (cmd.LotacaoId == Guid.Empty)
            return Result<FuncionarioDto>.Failure("VALIDACAO", "LotacaoId é obrigatório.");

        try
        {
            var cpf = new Cpf(cmd.Cpf);
            var existente = await _repo.GetByCpfHashAsync(cpf.Hash, ct);
            if (existente is not null)
                return Result<FuncionarioDto>.Failure("CPF_DUPLICADO", "Já existe um funcionário com este CPF.");

            var funcionario = new Funcionario(
                cmd.EmpresaId, cmd.Nome, cpf.Numero, cpf.Hash,
                cmd.DataAdmissao, cmd.CargoId, cmd.LotacaoId, cmd.SalarioBase,
                cmd.DataNascimento, cmd.Sexo, cmd.EstadoCivil, cmd.Nacionalidade,
                cmd.NomeMae, cmd.NomePai, cmd.TipoContrato, cmd.JornadaHorasSemanais);

            funcionario.EnderecoLogradouro = cmd.EnderecoLogradouro;
            funcionario.EnderecoNumero = cmd.EnderecoNumero;
            funcionario.EnderecoComplemento = cmd.EnderecoComplemento;
            funcionario.EnderecoBairro = cmd.EnderecoBairro;
            funcionario.EnderecoCep = cmd.EnderecoCep;
            funcionario.EnderecoMunicipio = cmd.EnderecoMunicipio;
            funcionario.EnderecoUf = cmd.EnderecoUf;
            funcionario.Telefone = cmd.Telefone;
            funcionario.Email = cmd.Email;

            await _repo.AddAsync(funcionario, ct);
            await _messageBus.PublishAsync(new FuncionarioCadastradoEvent(
                funcionario.Id, funcionario.EmpresaId, funcionario.CargoId,
                funcionario.SalarioBase, funcionario.DataAdmissao),
                "folha360.cadastros", "FuncionarioCadastrado", ct);

            return Result<FuncionarioDto>.Success(MapFuncionario(funcionario));
        }
        catch (ArgumentException ex)
        {
            return Result<FuncionarioDto>.Failure("VALIDACAO", ex.Message);
        }
    }

    private static FuncionarioDto MapFuncionario(Funcionario f) => new()
    {
        Id = f.Id,
        EmpresaId = f.EmpresaId,
        Nome = f.Nome,
        CpfMascarado = CpfMaskHelper.Mask(f.Cpf),
        DataNascimento = f.DataNascimento,
        Sexo = f.Sexo,
        EstadoCivil = f.EstadoCivil,
        Nacionalidade = f.Nacionalidade,
        NomeMae = f.NomeMae,
        NomePai = f.NomePai,
        DataAdmissao = f.DataAdmissao,
        DataDesligamento = f.DataDesligamento,
        Status = f.Status,
        CargoId = f.CargoId,
        LotacaoId = f.LotacaoId,
        SalarioBase = f.SalarioBase,
        TipoContrato = f.TipoContrato,
        JornadaHorasSemanais = f.JornadaHorasSemanais,
        CreatedAt = f.CreatedAt,
        UpdatedAt = f.UpdatedAt,
    };
}

public class AtualizarFuncionarioHandler : IRequestHandler<AtualizarFuncionarioCommand, Result<FuncionarioDto>>
{
    private readonly IFuncionarioRepository _repo;
    public AtualizarFuncionarioHandler(IFuncionarioRepository repo) => _repo = repo;

    public async Task<Result<FuncionarioDto>> Handle(AtualizarFuncionarioCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<FuncionarioDto>.Failure("VALIDACAO", "Id é obrigatório.");

        var funcionario = await _repo.GetByIdAsync(cmd.Id, ct);
        if (funcionario is null)
            return Result<FuncionarioDto>.Failure("NAO_ENCONTRADO", "Funcionário não encontrado.");

        funcionario.Atualizar(cmd.Nome, cmd.DataAdmissao, cmd.CargoId, cmd.LotacaoId, cmd.SalarioBase,
            cmd.DataNascimento, cmd.Sexo, cmd.EstadoCivil, cmd.Nacionalidade,
            cmd.NomeMae, cmd.NomePai, cmd.TipoContrato, cmd.JornadaHorasSemanais);

        funcionario.EnderecoLogradouro = cmd.EnderecoLogradouro;
        funcionario.EnderecoNumero = cmd.EnderecoNumero;
        funcionario.EnderecoComplemento = cmd.EnderecoComplemento;
        funcionario.EnderecoBairro = cmd.EnderecoBairro;
        funcionario.EnderecoCep = cmd.EnderecoCep;
        funcionario.EnderecoMunicipio = cmd.EnderecoMunicipio;
        funcionario.EnderecoUf = cmd.EnderecoUf;
        funcionario.Telefone = cmd.Telefone;
        funcionario.Email = cmd.Email;

        await _repo.UpdateAsync(funcionario, ct);

        return Result<FuncionarioDto>.Success(new FuncionarioDto
        {
            Id = funcionario.Id,
            EmpresaId = funcionario.EmpresaId,
            Nome = funcionario.Nome,
            CpfMascarado = "***",
            DataAdmissao = funcionario.DataAdmissao,
            Status = funcionario.Status,
            CargoId = funcionario.CargoId,
            LotacaoId = funcionario.LotacaoId,
            SalarioBase = funcionario.SalarioBase,
            CreatedAt = funcionario.CreatedAt,
            UpdatedAt = funcionario.UpdatedAt,
        });
    }
}

public class ExcluirFuncionarioHandler : IRequestHandler<ExcluirFuncionarioCommand, Result<bool>>
{
    private readonly IFuncionarioRepository _repo;
    public ExcluirFuncionarioHandler(IFuncionarioRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirFuncionarioCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterFuncionarioHandler : IRequestHandler<ObterFuncionarioQuery, Result<FuncionarioDto>>
{
    private readonly IFuncionarioRepository _repo;
    public ObterFuncionarioHandler(IFuncionarioRepository repo) => _repo = repo;

    public async Task<Result<FuncionarioDto>> Handle(ObterFuncionarioQuery query, CancellationToken ct)
    {
        var f = await _repo.GetByIdAsync(query.Id, ct);
        if (f is null)
            return Result<FuncionarioDto>.Failure("NAO_ENCONTRADO", "Funcionário não encontrado.");

        return Result<FuncionarioDto>.Success(new FuncionarioDto
        {
            Id = f.Id,
            EmpresaId = f.EmpresaId,
            Nome = f.Nome,
            CpfMascarado = CpfMaskHelper.Mask(f.Cpf),
            DataNascimento = f.DataNascimento,
            Sexo = f.Sexo,
            EstadoCivil = f.EstadoCivil,
            Nacionalidade = f.Nacionalidade,
            NomeMae = f.NomeMae,
            NomePai = f.NomePai,
            DataAdmissao = f.DataAdmissao,
            DataDesligamento = f.DataDesligamento,
            Status = f.Status,
            CargoId = f.CargoId,
            LotacaoId = f.LotacaoId,
            SalarioBase = f.SalarioBase,
            TipoContrato = f.TipoContrato,
            JornadaHorasSemanais = f.JornadaHorasSemanais,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt,
        });
    }
}

public class ListarFuncionariosHandler : IRequestHandler<ListarFuncionariosQuery, PaginatedResult<FuncionarioDto>>
{
    private readonly IFuncionarioRepository _repo;
    public ListarFuncionariosHandler(IFuncionarioRepository repo) => _repo = repo;

    public async Task<PaginatedResult<FuncionarioDto>> Handle(ListarFuncionariosQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy,
            query.EmpresaId, query.Status, query.CargoId, query.LotacaoId, query.Nome, ct);

        var dtos = items.Select(f => new FuncionarioDto
        {
            Id = f.Id,
            EmpresaId = f.EmpresaId,
            Nome = f.Nome,
            CpfMascarado = "***",
            DataAdmissao = f.DataAdmissao,
            Status = f.Status,
            CargoId = f.CargoId,
            LotacaoId = f.LotacaoId,
            SalarioBase = f.SalarioBase,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt,
        });

        return PaginatedResult<FuncionarioDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
