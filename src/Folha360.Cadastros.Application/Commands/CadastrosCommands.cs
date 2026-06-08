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
    public string? DescricaoAbreviada { get; init; }
    public bool EnviarEsocial { get; init; } = true;
    public bool IncideInss { get; init; }
    public bool IncideIrrf { get; init; }
    public bool IncideFgts { get; init; }
    public bool IncideContribuicaoSindical { get; init; }
    public bool IncideDecimoTerceiro { get; init; }
    public bool IncideFerias { get; init; }
    public bool IncideAvisoPrevio { get; init; }
    public bool IncideRescisao { get; init; }
    public bool IncideDissidio { get; init; }
    public bool IncideSalarioMaternidade { get; init; }
    public bool IncideAuxilioDoenca { get; init; }
    public bool IncideAdiantamento { get; init; }
    public string TipoCalculo { get; init; } = "VALOR_FIXO";
    public string? FormulaCalculo { get; init; }
    public decimal? ValorFixo { get; init; }
    public decimal? Percentual { get; init; }
    public Guid? RubricaBaseId { get; init; }
    public int OrdemCalculo { get; init; }
    public int OrdemExibicao { get; init; }
    public int? PrioridadeDesconto { get; init; }
    public decimal? TetoMaximo { get; init; }
    public decimal? PisoMinimo { get; init; }
    public bool Ativo { get; init; } = true;
    public DateTime? DataInicioVigencia { get; init; }
    public DateTime? DataFimVigencia { get; init; }
    public string? Observacao { get; init; }
    public Guid? GrupoRubricaId { get; init; }
}

public sealed record AtualizarRubricaCommand : IRequest<Result<RubricaDto>>
{
    public Guid Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string Natureza { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
    public string? DescricaoAbreviada { get; init; }
    public bool? EnviarEsocial { get; init; }
    public bool? IncideInss { get; init; }
    public bool? IncideIrrf { get; init; }
    public bool? IncideFgts { get; init; }
    public bool? IncideContribuicaoSindical { get; init; }
    public bool? IncideDecimoTerceiro { get; init; }
    public bool? IncideFerias { get; init; }
    public bool? IncideAvisoPrevio { get; init; }
    public bool? IncideRescisao { get; init; }
    public bool? IncideDissidio { get; init; }
    public bool? IncideSalarioMaternidade { get; init; }
    public bool? IncideAuxilioDoenca { get; init; }
    public bool? IncideAdiantamento { get; init; }
    public string? TipoCalculo { get; init; }
    public string? FormulaCalculo { get; init; }
    public decimal? ValorFixo { get; init; }
    public decimal? Percentual { get; init; }
    public Guid? RubricaBaseId { get; init; }
    public int? OrdemCalculo { get; init; }
    public int? OrdemExibicao { get; init; }
    public int? PrioridadeDesconto { get; init; }
    public decimal? TetoMaximo { get; init; }
    public decimal? PisoMinimo { get; init; }
    public bool? Ativo { get; init; }
    public DateTime? DataInicioVigencia { get; init; }
    public DateTime? DataFimVigencia { get; init; }
    public string? Observacao { get; init; }
    public Guid? GrupoRubricaId { get; init; }
}

public sealed record ExcluirRubricaCommand(Guid Id) : IRequest<Result<bool>>;

// ============================
// Rubrica — Simulação e Conformidade (ADR-006)
// ============================
public sealed record SimularRubricaCommand : IRequest<Result<SimulacaoResultadoDto>>
{
    public Guid EmpresaId { get; init; }
    public decimal SalarioBase { get; init; }
    public string? TipoContrato { get; init; }
    public decimal? QuantidadeHoras { get; init; }
    public decimal? QuantidadeDias { get; init; }
    public List<Guid> RubricasIds { get; init; } = new();
}

public sealed record VerificarConformidadeQuery : IRequest<Result<List<ConformidadeRubricaDto>>>
{
    public Guid EmpresaId { get; init; }
}

// ============================
// Processo Administrativo (S-1070)
// ============================
public sealed record CriarProcessoAdministrativoCommand : IRequest<Result<ProcessoAdministrativoDto>>
{
    public Guid EmpresaId { get; init; }
    public string NumeroProcesso { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public string? Orgao { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public string? Observacao { get; init; }
}

public sealed record VincularRubricaProcessoCommand : IRequest<Result<bool>>
{
    public Guid ProcessoAdministrativoId { get; init; }
    public Guid RubricaId { get; init; }
}

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

// ============================
// Subsistema de Rubricas (ADR-006)
// ============================

// --- GrupoRubrica ---
public sealed record CriarGrupoRubricaCommand : IRequest<Result<GrupoRubricaDto>>
{
    public Guid EmpresaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Natureza { get; init; } = string.Empty;
    public int OrdemExibicao { get; init; }
}

public sealed record AtualizarGrupoRubricaCommand : IRequest<Result<GrupoRubricaDto>>
{
    public Guid Id { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string Natureza { get; init; } = string.Empty;
    public int? OrdemExibicao { get; init; }
}

public sealed record ExcluirGrupoRubricaCommand(Guid Id) : IRequest<Result<bool>>;

// --- RubricaComposicao ---
public sealed record AdicionarComponenteCommand : IRequest<Result<RubricaComposicaoDto>>
{
    public Guid RubricaPrincipalId { get; init; }
    public Guid RubricaComponenteId { get; init; }
    public string Operador { get; init; } = "+";
    public decimal? PercentualComposicao { get; init; }
    public int Ordem { get; init; }
    public bool Obrigatorio { get; init; } = true;
}

public sealed record RemoverComponenteCommand(Guid RubricaPrincipalId, Guid ComposicaoId) : IRequest<Result<bool>>;

// --- RubricaFormula ---
public sealed record CriarRubricaFormulaCommand : IRequest<Result<RubricaFormulaDto>>
{
    public Guid RubricaId { get; init; }
    public string Expressao { get; init; } = string.Empty;
    public string? Parametros { get; init; }
    public string? DescricaoFormal { get; init; }
}

public sealed record AtualizarRubricaFormulaCommand : IRequest<Result<RubricaFormulaDto>>
{
    public Guid RubricaId { get; init; }
    public string Expressao { get; init; } = string.Empty;
    public string? Parametros { get; init; }
    public string? DescricaoFormal { get; init; }
}

// --- RubricaTabelaProgressiva ---
public sealed record CriarFaixaProgressivaCommand : IRequest<Result<RubricaTabelaProgressivaDto>>
{
    public Guid RubricaId { get; init; }
    public int AnoVigencia { get; init; }
    public decimal FaixaDe { get; init; }
    public decimal? FaixaAte { get; init; }
    public decimal Aliquota { get; init; }
    public decimal Deducao { get; init; }
    public int Ordem { get; init; }
}

public sealed record AtualizarFaixaProgressivaCommand : IRequest<Result<RubricaTabelaProgressivaDto>>
{
    public Guid Id { get; init; }
    public int AnoVigencia { get; init; }
    public decimal FaixaDe { get; init; }
    public decimal? FaixaAte { get; init; }
    public decimal Aliquota { get; init; }
    public decimal Deducao { get; init; }
    public int Ordem { get; init; }
}

public sealed record ExcluirFaixaProgressivaCommand(Guid Id) : IRequest<Result<bool>>;

// --- RubricaIncidencia ---
public sealed record AdicionarIncidenciaCommand : IRequest<Result<RubricaIncidenciaDto>>
{
    public Guid RubricaId { get; init; }
    public string TipoIncidencia { get; init; } = string.Empty;
}

public sealed record RemoverIncidenciaCommand(Guid RubricaId, Guid IncidenciaId) : IRequest<Result<bool>>;
