using Folha360.Domain;
using Folha360.Domain.Attributes;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade DadosBancariosFuncionario — contas bancárias do funcionário para pagamento.
/// Schema: tenant.
/// </summary>
public class DadosBancariosFuncionario : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid BancoId { get; private set; }
    public string Agencia { get; private set; } = null!;
    public string? AgenciaDv { get; private set; }
    public string Conta { get; private set; } = null!;
    public string? ContaDv { get; private set; }
    public string TipoConta { get; private set; } = null!;

    [SensitiveData]
    public string? ChavePix { get; private set; }

    public bool ContaPrincipal { get; private set; }

    private DadosBancariosFuncionario()
        : base()
    {
    }

    public DadosBancariosFuncionario(
        Guid funcionarioId,
        Guid bancoId,
        string agencia,
        string conta,
        string tipoConta,
        string? agenciaDv = null,
        string? contaDv = null,
        string? chavePix = null,
        bool contaPrincipal = false)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        BancoId = bancoId;
        Agencia = agencia;
        AgenciaDv = agenciaDv;
        Conta = conta;
        ContaDv = contaDv;
        TipoConta = tipoConta;
        ChavePix = chavePix;
        ContaPrincipal = contaPrincipal;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string agencia,
        string conta,
        string tipoConta,
        string? agenciaDv = null,
        string? contaDv = null,
        string? chavePix = null,
        bool? contaPrincipal = null)
    {
        Agencia = agencia;
        Conta = conta;
        TipoConta = tipoConta;
        AgenciaDv = agenciaDv;
        ContaDv = contaDv;
        ChavePix = chavePix;

        if (contaPrincipal.HasValue)
        {
            ContaPrincipal = contaPrincipal.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }
}
