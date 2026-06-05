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

// ============================
// Empresa Handlers
// ============================
public class CriarEmpresaHandler : IRequestHandler<CriarEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repo;
    private readonly ITenantContext _tenantContext;
    private readonly IMessageBus _messageBus;

    public CriarEmpresaHandler(IEmpresaRepository repo, ITenantContext tenantContext, IMessageBus messageBus)
    {
        _repo = repo;
        _tenantContext = tenantContext;
        _messageBus = messageBus;
    }

    public async Task<Result<EmpresaDto>> Handle(CriarEmpresaCommand cmd, CancellationToken ct)
    {
        try
        {
            var cnpj = new Cnpj(cmd.Cnpj);
            var existente = await _repo.GetByCnpjAsync(cnpj.Numero, ct);
            if (existente is not null)
                return Result<EmpresaDto>.Failure("CNPJ_DUPLICADO", "Já existe uma empresa com este CNPJ.");

            var empresa = new Empresa(
                Guid.Parse(_tenantContext.TenantId), cnpj.Numero, cmd.RazaoSocial, cmd.RegimeTributario,
                cmd.NomeFantasia, cmd.Cnae, cmd.Fpas, cmd.CodigoTerceiros,
                cmd.ClassificacaoTributaria, cmd.MatrizFilial, cmd.CnpjMatriz);

            empresa.EnderecoLogradouro = cmd.EnderecoLogradouro;
            empresa.EnderecoNumero = cmd.EnderecoNumero;
            empresa.EnderecoComplemento = cmd.EnderecoComplemento;
            empresa.EnderecoBairro = cmd.EnderecoBairro;
            empresa.EnderecoCep = cmd.EnderecoCep;
            empresa.EnderecoMunicipio = cmd.EnderecoMunicipio;
            empresa.EnderecoUf = cmd.EnderecoUf;
            empresa.Telefone = cmd.Telefone;
            empresa.Email = cmd.Email;

            await _repo.AddAsync(empresa, ct);
            await _messageBus.PublishAsync(new EmpresaCadastradaEvent(
                empresa.Id, empresa.Cnpj, empresa.RazaoSocial, empresa.RegimeTributario),
                "folha360.cadastros", "EmpresaCadastrada", ct);

            return Result<EmpresaDto>.Success(MapEmpresa(empresa));
        }
        catch (ArgumentException ex)
        {
            return Result<EmpresaDto>.Failure("VALIDACAO", ex.Message);
        }
    }

    private static EmpresaDto MapEmpresa(Empresa e) => new()
    {
        Id = e.Id,
        TenantId = e.TenantId,
        Cnpj = e.Cnpj,
        RazaoSocial = e.RazaoSocial,
        NomeFantasia = e.NomeFantasia,
        Cnae = e.Cnae,
        RegimeTributario = e.RegimeTributario,
        Fpas = e.Fpas,
        CodigoTerceiros = e.CodigoTerceiros,
        ClassificacaoTributaria = e.ClassificacaoTributaria,
        MatrizFilial = e.MatrizFilial,
        CnpjMatriz = e.CnpjMatriz,
        EnderecoLogradouro = e.EnderecoLogradouro,
        EnderecoNumero = e.EnderecoNumero,
        EnderecoComplemento = e.EnderecoComplemento,
        EnderecoBairro = e.EnderecoBairro,
        EnderecoCep = e.EnderecoCep,
        EnderecoMunicipio = e.EnderecoMunicipio,
        EnderecoUf = e.EnderecoUf,
        Telefone = e.Telefone,
        Email = e.Email,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}

public class AtualizarEmpresaHandler : IRequestHandler<AtualizarEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repo;

    public AtualizarEmpresaHandler(IEmpresaRepository repo) => _repo = repo;

    public async Task<Result<EmpresaDto>> Handle(AtualizarEmpresaCommand cmd, CancellationToken ct)
    {
        var empresa = await _repo.GetByIdAsync(cmd.Id, ct);
        if (empresa is null)
            return Result<EmpresaDto>.Failure("NAO_ENCONTRADO", "Empresa não encontrada.");

        empresa.Atualizar(cmd.RazaoSocial, cmd.RegimeTributario, cmd.NomeFantasia, cmd.Cnae,
            cmd.Fpas, cmd.CodigoTerceiros, cmd.ClassificacaoTributaria, cmd.MatrizFilial, cmd.CnpjMatriz);

        empresa.EnderecoLogradouro = cmd.EnderecoLogradouro;
        empresa.EnderecoNumero = cmd.EnderecoNumero;
        empresa.EnderecoComplemento = cmd.EnderecoComplemento;
        empresa.EnderecoBairro = cmd.EnderecoBairro;
        empresa.EnderecoCep = cmd.EnderecoCep;
        empresa.EnderecoMunicipio = cmd.EnderecoMunicipio;
        empresa.EnderecoUf = cmd.EnderecoUf;
        empresa.Telefone = cmd.Telefone;
        empresa.Email = cmd.Email;

        await _repo.UpdateAsync(empresa, ct);

        return Result<EmpresaDto>.Success(new EmpresaDto
        {
            Id = empresa.Id,
            TenantId = empresa.TenantId,
            Cnpj = empresa.Cnpj,
            RazaoSocial = empresa.RazaoSocial,
            NomeFantasia = empresa.NomeFantasia,
            Cnae = empresa.Cnae,
            RegimeTributario = empresa.RegimeTributario,
            Fpas = empresa.Fpas,
            CreatedAt = empresa.CreatedAt,
            UpdatedAt = empresa.UpdatedAt,
        });
    }
}

public class ExcluirEmpresaHandler : IRequestHandler<ExcluirEmpresaCommand, Result<bool>>
{
    private readonly IEmpresaRepository _repo;
    public ExcluirEmpresaHandler(IEmpresaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirEmpresaCommand cmd, CancellationToken ct)
    {
        var empresa = await _repo.GetByIdAsync(cmd.Id, ct);
        if (empresa is null)
            return Result<bool>.Failure("NAO_ENCONTRADO", "Empresa não encontrada.");

        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterEmpresaHandler : IRequestHandler<ObterEmpresaQuery, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repo;
    public ObterEmpresaHandler(IEmpresaRepository repo) => _repo = repo;

    public async Task<Result<EmpresaDto>> Handle(ObterEmpresaQuery query, CancellationToken ct)
    {
        var empresa = await _repo.GetByIdAsync(query.Id, ct);
        if (empresa is null)
            return Result<EmpresaDto>.Failure("NAO_ENCONTRADO", "Empresa não encontrada.");

        return Result<EmpresaDto>.Success(new EmpresaDto
        {
            Id = empresa.Id,
            TenantId = empresa.TenantId,
            Cnpj = empresa.Cnpj,
            RazaoSocial = empresa.RazaoSocial,
            NomeFantasia = empresa.NomeFantasia,
            Cnae = empresa.Cnae,
            RegimeTributario = empresa.RegimeTributario,
            Fpas = empresa.Fpas,
            CodigoTerceiros = empresa.CodigoTerceiros,
            ClassificacaoTributaria = empresa.ClassificacaoTributaria,
            MatrizFilial = empresa.MatrizFilial,
            CnpjMatriz = empresa.CnpjMatriz,
            EnderecoLogradouro = empresa.EnderecoLogradouro,
            EnderecoNumero = empresa.EnderecoNumero,
            EnderecoComplemento = empresa.EnderecoComplemento,
            EnderecoBairro = empresa.EnderecoBairro,
            EnderecoCep = empresa.EnderecoCep,
            EnderecoMunicipio = empresa.EnderecoMunicipio,
            EnderecoUf = empresa.EnderecoUf,
            Telefone = empresa.Telefone,
            Email = empresa.Email,
            CreatedAt = empresa.CreatedAt,
            UpdatedAt = empresa.UpdatedAt,
        });
    }
}

public class ListarEmpresasHandler : IRequestHandler<ListarEmpresasQuery, PaginatedResult<EmpresaDto>>
{
    private readonly IEmpresaRepository _repo;
    public ListarEmpresasHandler(IEmpresaRepository repo) => _repo = repo;

    public async Task<PaginatedResult<EmpresaDto>> Handle(ListarEmpresasQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy,
            query.Cnpj, query.RazaoSocial, query.RegimeTributario, ct);

        var dtos = items.Select(e => new EmpresaDto
        {
            Id = e.Id,
            TenantId = e.TenantId,
            Cnpj = e.Cnpj,
            RazaoSocial = e.RazaoSocial,
            NomeFantasia = e.NomeFantasia,
            Cnae = e.Cnae,
            RegimeTributario = e.RegimeTributario,
            Fpas = e.Fpas,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
        });

        return PaginatedResult<EmpresaDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}

// ============================
// Funcionario Handlers
// ============================
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
        CpfMascarado = MaskCpf(f.Cpf),
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

    private static string MaskCpf(string cpf)
        => cpf.Length == 11 ? $"***.{cpf[3..6]}.{cpf[6..9]}-**" : "***";
}

public class AtualizarFuncionarioHandler : IRequestHandler<AtualizarFuncionarioCommand, Result<FuncionarioDto>>
{
    private readonly IFuncionarioRepository _repo;
    public AtualizarFuncionarioHandler(IFuncionarioRepository repo) => _repo = repo;

    public async Task<Result<FuncionarioDto>> Handle(AtualizarFuncionarioCommand cmd, CancellationToken ct)
    {
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
            CpfMascarado = f.Cpf.Length == 11 ? $"***.{f.Cpf[3..6]}.{f.Cpf[6..9]}-**" : "***",
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

// ============================
// Cargo Handlers
// ============================
public class CriarCargoHandler : IRequestHandler<CriarCargoCommand, Result<CargoDto>>
{
    private readonly ICargoRepository _repo;
    public CriarCargoHandler(ICargoRepository repo) => _repo = repo;

    public async Task<Result<CargoDto>> Handle(CriarCargoCommand cmd, CancellationToken ct)
    {
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

// ============================
// Rubrica Handlers
// ============================
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

// ============================
// Lotacao Handlers
// ============================
public class CriarLotacaoHandler : IRequestHandler<CriarLotacaoCommand, Result<LotacaoDto>>
{
    private readonly ILotacaoRepository _repo;
    public CriarLotacaoHandler(ILotacaoRepository repo) => _repo = repo;

    public async Task<Result<LotacaoDto>> Handle(CriarLotacaoCommand cmd, CancellationToken ct)
    {
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
