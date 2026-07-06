using Folha360.Domain;
using Folha360.Domain.Attributes;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade ConfiguracaoBancariaEmpresa — contas bancárias da empresa para folha, tributos, etc.
/// Schema: tenant.
/// </summary>
public class ConfiguracaoBancariaEmpresa : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public Guid BancoId { get; private set; }
    public string Agencia { get; private set; } = null!;
    public string? AgenciaDv { get; private set; }
    public string Conta { get; private set; } = null!;
    public string? ContaDv { get; private set; }
    public string TipoConta { get; private set; } = null!;

    [SensitiveData]
    public string? ChavePix { get; private set; }

    public string Finalidade { get; private set; } = null!;
    public bool Ativa { get; private set; } = true;

    private ConfiguracaoBancariaEmpresa()
        : base()
    {
    }

    public ConfiguracaoBancariaEmpresa(
        Guid empresaId,
        Guid bancoId,
        string agencia,
        string conta,
        string tipoConta,
        string finalidade,
        string? agenciaDv = null,
        string? contaDv = null,
        string? chavePix = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        BancoId = bancoId;
        Agencia = agencia;
        AgenciaDv = agenciaDv;
        Conta = conta;
        ContaDv = contaDv;
        TipoConta = tipoConta;
        ChavePix = chavePix;
        Finalidade = finalidade;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string agencia,
        string conta,
        string tipoConta,
        string finalidade,
        string? agenciaDv = null,
        string? contaDv = null,
        string? chavePix = null,
        bool? ativa = null)
    {
        Agencia = agencia;
        Conta = conta;
        TipoConta = tipoConta;
        Finalidade = finalidade;
        AgenciaDv = agenciaDv;
        ContaDv = contaDv;
        ChavePix = chavePix;
        if (ativa.HasValue)
        {
            Ativa = ativa.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
