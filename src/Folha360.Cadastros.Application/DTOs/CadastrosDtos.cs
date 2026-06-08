namespace Folha360.Cadastros.Application.DTOs;

public sealed record EmpresaDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
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
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record FuncionarioDto
{
    public Guid Id { get; init; }
    public Guid EmpresaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string CpfMascarado { get; init; } = string.Empty;
    public DateOnly? DataNascimento { get; init; }
    public string? Sexo { get; init; }
    public string? EstadoCivil { get; init; }
    public string? Nacionalidade { get; init; }
    public string? NomeMae { get; init; }
    public string? NomePai { get; init; }
    public DateOnly DataAdmissao { get; init; }
    public DateOnly? DataDesligamento { get; init; }
    public string Status { get; init; } = "Ativo";
    public Guid CargoId { get; init; }
    public Guid LotacaoId { get; init; }
    public decimal SalarioBase { get; init; }
    public string? TipoContrato { get; init; }
    public int? JornadaHorasSemanais { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record CargoDto
{
    public Guid Id { get; init; }
    public Guid EmpresaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Cbo { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public decimal? SalarioBaseMinimo { get; init; }
    public decimal? SalarioBaseMaximo { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record RubricaDto
{
    public Guid Id { get; init; }
    public Guid EmpresaId { get; init; }
    public Guid? GrupoRubricaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? DescricaoAbreviada { get; init; }
    public string Natureza { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
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
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed record LotacaoDto
{
    public Guid Id { get; init; }
    public Guid EmpresaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

// ============================
// Simulação e Conformidade (ADR-006)
// ============================
public sealed record SimulacaoResultadoDto
{
    public Dictionary<Guid, decimal> ValoresPorRubrica { get; init; } = new();
    public decimal TotalVencimentos { get; init; }
    public decimal TotalDescontos { get; init; }
    public decimal Liquido { get; init; }
    public decimal BaseInss { get; init; }
    public decimal BaseIrrf { get; init; }
    public decimal BaseFgts { get; init; }
    public List<string> Erros { get; init; } = new();
}

public sealed record ConformidadeRubricaDto
{
    public Guid RubricaId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string? TipoEsocial { get; init; }
    public string Problema { get; init; } = string.Empty;
}

public sealed record ProcessoAdministrativoDto
{
    public Guid Id { get; init; }
    public Guid EmpresaId { get; init; }
    public string NumeroProcesso { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public string? Orgao { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public string? Observacao { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
