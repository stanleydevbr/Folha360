using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Empresa — representa um contribuinte/empregador associado a um tenant.
/// Schema: public (compartilhado entre tenants).
/// </summary>
public class Empresa : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string Cnpj { get; private set; } = null!;
    public string RazaoSocial { get; private set; } = null!;
    public string? NomeFantasia { get; private set; }
    public string? Cnae { get; private set; }
    public string RegimeTributario { get; private set; } = null!;
    public string? Fpas { get; private set; }
    public string? CodigoTerceiros { get; private set; }
    public string? ClassificacaoTributaria { get; private set; }
    public string? MatrizFilial { get; private set; }
    public string? CnpjMatriz { get; private set; }
    public string? EnderecoLogradouro { get; set; }
    public string? EnderecoNumero { get; set; }
    public string? EnderecoComplemento { get; set; }
    public string? EnderecoBairro { get; set; }
    public string? EnderecoCep { get; set; }
    public string? EnderecoMunicipio { get; set; }
    public string? EnderecoUf { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }

    private Empresa()
    {
    }

    public Empresa(
        Guid tenantId,
        string cnpj,
        string razaoSocial,
        string regimeTributario,
        string? nomeFantasia = null,
        string? cnae = null,
        string? fpas = null,
        string? codigoTerceiros = null,
        string? classificacaoTributaria = null,
        string? matrizFilial = null,
        string? cnpjMatriz = null)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Cnpj = cnpj;
        RazaoSocial = razaoSocial;
        RegimeTributario = regimeTributario;
        NomeFantasia = nomeFantasia;
        Cnae = cnae;
        Fpas = fpas;
        CodigoTerceiros = codigoTerceiros;
        ClassificacaoTributaria = classificacaoTributaria;
        MatrizFilial = matrizFilial;
        CnpjMatriz = cnpjMatriz;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string razaoSocial,
        string regimeTributario,
        string? nomeFantasia = null,
        string? cnae = null,
        string? fpas = null,
        string? codigoTerceiros = null,
        string? classificacaoTributaria = null,
        string? matrizFilial = null,
        string? cnpjMatriz = null)
    {
        RazaoSocial = razaoSocial;
        RegimeTributario = regimeTributario;
        NomeFantasia = nomeFantasia;
        Cnae = cnae;
        Fpas = fpas;
        CodigoTerceiros = codigoTerceiros;
        ClassificacaoTributaria = classificacaoTributaria;
        MatrizFilial = matrizFilial;
        CnpjMatriz = cnpjMatriz;
        UpdatedAt = DateTime.UtcNow;
    }
}
