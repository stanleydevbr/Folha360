using System.Security.Cryptography;
using System.Text;
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
        var tenantId = ResolveTenantGuid(_tenantContext.TenantId);
        if (tenantId == Guid.Empty)
            return Result<EmpresaDto>.Failure("VALIDACAO", "TenantId inválido.");

        try
        {
            var cnpj = new Cnpj(cmd.Cnpj);
            var existente = await _repo.GetByCnpjAsync(cnpj.Numero, ct);
            if (existente is not null)
                return Result<EmpresaDto>.Failure("CNPJ_DUPLICADO", "Já existe uma empresa com este CNPJ.");

            var empresa = new Empresa(
                tenantId, cnpj.Numero, cmd.RazaoSocial, cmd.RegimeTributario,
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

    /// <summary>
    /// Converte um TenantId string (ex.: "demo") em GUID determinístico via MD5.
    /// Se a string já for um GUID válido, faz parse direto.
    /// </summary>
    private static Guid ResolveTenantGuid(string tenantId)
    {
        if (string.IsNullOrEmpty(tenantId))
            return Guid.Empty;

        // Se já for GUID, parse direto
        if (Guid.TryParse(tenantId, out var guid))
            return guid;

        // Gera GUID determinístico a partir da string (MD5)
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(tenantId));
        return new Guid(hash);
    }
}

public class AtualizarEmpresaHandler : IRequestHandler<AtualizarEmpresaCommand, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repo;

    public AtualizarEmpresaHandler(IEmpresaRepository repo) => _repo = repo;

    public async Task<Result<EmpresaDto>> Handle(AtualizarEmpresaCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<EmpresaDto>.Failure("VALIDACAO", "Id é obrigatório.");

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
        if (cmd.Id == Guid.Empty)
            return Result<bool>.Failure("VALIDACAO", "Id é obrigatório.");

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
