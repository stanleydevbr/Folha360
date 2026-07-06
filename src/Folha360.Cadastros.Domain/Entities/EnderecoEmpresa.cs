using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade EnderecoEmpresa — múltiplos endereços por tipo (Principal, Fiscal, Cobrança, etc.).
/// Schema: tenant.
/// </summary>
public class EnderecoEmpresa : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Tipo { get; private set; } = null!;
    public string Logradouro { get; private set; } = null!;
    public string? Numero { get; private set; }
    public string? Complemento { get; private set; }
    public string? Bairro { get; private set; }
    public string? Cep { get; private set; }
    public Guid? MunicipioId { get; private set; }
    public string? Uf { get; private set; }
    public bool Estrangeiro { get; private set; }
    public string? Pais { get; private set; }

    private EnderecoEmpresa()
        : base()
    {
    }

    public EnderecoEmpresa(
        Guid empresaId,
        string tipo,
        string logradouro,
        string? numero = null,
        string? complemento = null,
        string? bairro = null,
        string? cep = null,
        Guid? municipioId = null,
        string? uf = null,
        bool estrangeiro = false,
        string? pais = null)
    {
        if (estrangeiro && string.IsNullOrWhiteSpace(pais))
        {
            throw new ArgumentException("País é obrigatório para endereço estrangeiro.");
        }

        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Tipo = tipo;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        MunicipioId = municipioId;
        Uf = uf;
        Estrangeiro = estrangeiro;
        Pais = pais;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string logradouro,
        string? numero = null,
        string? complemento = null,
        string? bairro = null,
        string? cep = null,
        Guid? municipioId = null,
        string? uf = null,
        bool? estrangeiro = null,
        string? pais = null)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        MunicipioId = municipioId;
        Uf = uf;

        if (estrangeiro.HasValue)
        {
            Estrangeiro = estrangeiro.Value;
        }

        if (Estrangeiro && string.IsNullOrWhiteSpace(pais))
        {
            throw new ArgumentException("País é obrigatório para endereço estrangeiro.");
        }

        Pais = pais;
        UpdatedAt = DateTime.UtcNow;
    }
}
