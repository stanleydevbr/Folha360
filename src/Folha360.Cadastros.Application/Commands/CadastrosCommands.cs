using Folha360.Cadastros.Application.DTOs;
using MediatR;

namespace Folha360.Cadastros.Application.Commands;

// ============================
// Empresa
// ============================
public sealed record CriarEmpresaCommand : IRequest<Result<EmpresaDto>>
{
    public string Cnpj { get; init; } = string.Empty;
    public string RazaoSocial { get; init; } = string.Empty;
    public string? NomeFantasia { get; init; }
    public string? Cnae { get; init; }
    public string RegimeTributario { get; init; } = string.Empty;
    public string? Fpas { get; init; }
    public string? CodigoTerceiros { get; init; }
    public string? ClassificacaoTributaria { get; init; }
    public string? MatrizFilial { get; init; }
    public string? CnpjMatriz { get; init; }
    public string? EnderecoLogradouro { get; init; }
    public string? EnderecoNumero { get; init; }
    public string? EnderecoComplemento { get; init; }
    public string? EnderecoBairro { get; init; }
    public string? EnderecoCep { get; init; }
    public string? EnderecoMunicipio { get; init; }
    public string? EnderecoUf { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
}

public sealed record AtualizarEmpresaCommand : IRequest<Result<EmpresaDto>>
{
    public Guid Id { get; init; }
    public string RazaoSocial { get; init; } = string.Empty;
    public string RegimeTributario { get; init; } = string.Empty;
    public string? NomeFantasia { get; init; }
    public string? Cnae { get; init; }
    public string? Fpas { get; init; }
    public string? CodigoTerceiros { get; init; }
    public string? ClassificacaoTributaria { get; init; }
    public string? MatrizFilial { get; init; }
    public string? CnpjMatriz { get; init; }
    public string? EnderecoLogradouro { get; init; }
    public string? EnderecoNumero { get; init; }
    public string? EnderecoComplemento { get; init; }
    public string? EnderecoBairro { get; init; }
    public string? EnderecoCep { get; init; }
    public string? EnderecoMunicipio { get; init; }
    public string? EnderecoUf { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
}

public sealed record ExcluirEmpresaCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Funcionario
// ============================
public sealed record CriarFuncionarioCommand : IRequest<Result<FuncionarioDto>>
{
    public Guid EmpresaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public DateOnly DataAdmissao { get; init; }
    public Guid CargoId { get; init; }
    public Guid LotacaoId { get; init; }
    public decimal SalarioBase { get; init; }
    public DateOnly? DataNascimento { get; init; }
    public string? Sexo { get; init; }
    public string? EstadoCivil { get; init; }
    public string? Nacionalidade { get; init; }
    public string? NomeMae { get; init; }
    public string? NomePai { get; init; }
    public string? TipoContrato { get; init; }
    public int? JornadaHorasSemanais { get; init; }
    public string? EnderecoLogradouro { get; init; }
    public string? EnderecoNumero { get; init; }
    public string? EnderecoComplemento { get; init; }
    public string? EnderecoBairro { get; init; }
    public string? EnderecoCep { get; init; }
    public string? EnderecoMunicipio { get; init; }
    public string? EnderecoUf { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
}

public sealed record AtualizarFuncionarioCommand : IRequest<Result<FuncionarioDto>>
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public DateOnly DataAdmissao { get; init; }
    public Guid CargoId { get; init; }
    public Guid LotacaoId { get; init; }
    public decimal SalarioBase { get; init; }
    public DateOnly? DataNascimento { get; init; }
    public string? Sexo { get; init; }
    public string? EstadoCivil { get; init; }
    public string? Nacionalidade { get; init; }
    public string? NomeMae { get; init; }
    public string? NomePai { get; init; }
    public string? TipoContrato { get; init; }
    public int? JornadaHorasSemanais { get; init; }
    public string? EnderecoLogradouro { get; init; }
    public string? EnderecoNumero { get; init; }
    public string? EnderecoComplemento { get; init; }
    public string? EnderecoBairro { get; init; }
    public string? EnderecoCep { get; init; }
    public string? EnderecoMunicipio { get; init; }
    public string? EnderecoUf { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
}

public sealed record ExcluirFuncionarioCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Cargo
// ============================
public sealed record CriarCargoCommand : IRequest<Result<CargoDto>>
{
    public Guid EmpresaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cbo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal? SalarioBaseMinimo { get; init; }
    public decimal? SalarioBaseMaximo { get; init; }
}

public sealed record AtualizarCargoCommand : IRequest<Result<CargoDto>>
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cbo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal? SalarioBaseMinimo { get; init; }
    public decimal? SalarioBaseMaximo { get; init; }
}

public sealed record ExcluirCargoCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Rubrica
// ============================
public sealed record CriarRubricaCommand : IRequest<Result<RubricaDto>>
{
    public Guid EmpresaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Natureza { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
    public bool IncideInss { get; init; }
    public bool IncideIrrf { get; init; }
    public bool IncideFgts { get; init; }
    public bool IncideContribuicaoSindical { get; init; }
    public bool IncideDecimoTerceiro { get; init; }
    public bool IncideFerias { get; init; }
    public bool IncideAvisoPrevio { get; init; }
    public string? FormulaCalculo { get; init; }
    public int OrdemExibicao { get; init; }
}

public sealed record AtualizarRubricaCommand : IRequest<Result<RubricaDto>>
{
    public Guid Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string Natureza { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
    public bool? IncideInss { get; init; }
    public bool? IncideIrrf { get; init; }
    public bool? IncideFgts { get; init; }
    public bool? IncideContribuicaoSindical { get; init; }
    public bool? IncideDecimoTerceiro { get; init; }
    public bool? IncideFerias { get; init; }
    public bool? IncideAvisoPrevio { get; init; }
    public string? FormulaCalculo { get; init; }
    public int? OrdemExibicao { get; init; }
}

public sealed record ExcluirRubricaCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Lotacao
// ============================
public sealed record CriarLotacaoCommand : IRequest<Result<LotacaoDto>>
{
    public Guid EmpresaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
}

public sealed record AtualizarLotacaoCommand : IRequest<Result<LotacaoDto>>
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
}

public sealed record ExcluirLotacaoCommand(Guid Id) : IRequest<Result<bool>>;
