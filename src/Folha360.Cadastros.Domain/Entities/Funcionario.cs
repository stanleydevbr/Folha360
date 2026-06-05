using Folha360.Cadastros.Domain.Attributes;
using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Funcionario — entidade central do sistema.
/// Schema: tenant.
/// </summary>
public class Funcionario : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Nome { get; private set; } = null!;

    [SensitiveData]
    public string Cpf { get; private set; } = null!;
    public string CpfHash { get; private set; } = null!;

    public DateOnly? DataNascimento { get; private set; }
    public string? Sexo { get; private set; }
    public string? EstadoCivil { get; private set; }
    public string? Nacionalidade { get; private set; }
    public string? NomeMae { get; private set; }
    public string? NomePai { get; private set; }
    public string? EnderecoLogradouro { get; set; }
    public string? EnderecoNumero { get; set; }
    public string? EnderecoComplemento { get; set; }
    public string? EnderecoBairro { get; set; }
    public string? EnderecoCep { get; set; }
    public string? EnderecoMunicipio { get; set; }
    public string? EnderecoUf { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public DateOnly DataAdmissao { get; private set; }
    public DateOnly? DataDesligamento { get; private set; }
    public string Status { get; private set; } = "Ativo";
    public Guid CargoId { get; private set; }
    public Guid LotacaoId { get; private set; }
    public decimal SalarioBase { get; private set; }
    public string? TipoContrato { get; private set; }
    public int? JornadaHorasSemanais { get; private set; }

    private Funcionario()
    {
    }

    public Funcionario(
        Guid empresaId,
        string nome,
        string cpf,
        string cpfHash,
        DateOnly dataAdmissao,
        Guid cargoId,
        Guid lotacaoId,
        decimal salarioBase,
        DateOnly? dataNascimento = null,
        string? sexo = null,
        string? estadoCivil = null,
        string? nacionalidade = null,
        string? nomeMae = null,
        string? nomePai = null,
        string? tipoContrato = null,
        int? jornadaHorasSemanais = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Nome = nome;
        Cpf = cpf;
        CpfHash = cpfHash;
        DataAdmissao = dataAdmissao;
        CargoId = cargoId;
        LotacaoId = lotacaoId;
        SalarioBase = salarioBase;
        DataNascimento = dataNascimento;
        Sexo = sexo;
        EstadoCivil = estadoCivil;
        Nacionalidade = nacionalidade;
        NomeMae = nomeMae;
        NomePai = nomePai;
        TipoContrato = tipoContrato;
        JornadaHorasSemanais = jornadaHorasSemanais;
        Status = "Ativo";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        DateOnly dataAdmissao,
        Guid cargoId,
        Guid lotacaoId,
        decimal salarioBase,
        DateOnly? dataNascimento = null,
        string? sexo = null,
        string? estadoCivil = null,
        string? nacionalidade = null,
        string? nomeMae = null,
        string? nomePai = null,
        string? tipoContrato = null,
        int? jornadaHorasSemanais = null)
    {
        Nome = nome;
        DataAdmissao = dataAdmissao;
        CargoId = cargoId;
        LotacaoId = lotacaoId;
        SalarioBase = salarioBase;
        DataNascimento = dataNascimento;
        Sexo = sexo;
        EstadoCivil = estadoCivil;
        Nacionalidade = nacionalidade;
        NomeMae = nomeMae;
        NomePai = nomePai;
        TipoContrato = tipoContrato;
        JornadaHorasSemanais = jornadaHorasSemanais;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desligar(DateOnly dataDesligamento)
    {
        Status = "Desligado";
        DataDesligamento = dataDesligamento;
        UpdatedAt = DateTime.UtcNow;
    }
}
