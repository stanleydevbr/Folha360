using Folha360.Cadastros.Domain.Attributes;
using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Documento — documentos do funcionário (CTPS, PIS/PASEP, RG, etc.).
/// Schema: tenant. UNIQUE(funcionario_id, tipo).
/// </summary>
public class Documento : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public string Tipo { get; private set; } = null!;

    [SensitiveData]
    public string Numero { get; private set; } = null!;

    public DateOnly? DataEmissao { get; private set; }
    public DateOnly? DataValidade { get; private set; }
    public string? OrgaoEmissor { get; private set; }
    public string? UfEmissor { get; private set; }
    public string? ArquivoPath { get; private set; }

    private Documento()
    {
    }

    public Documento(
        Guid funcionarioId,
        string tipo,
        string numero,
        DateOnly? dataEmissao = null,
        DateOnly? dataValidade = null,
        string? orgaoEmissor = null,
        string? ufEmissor = null,
        string? arquivoPath = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        Tipo = tipo;
        Numero = numero;
        DataEmissao = dataEmissao;
        DataValidade = dataValidade;
        OrgaoEmissor = orgaoEmissor;
        UfEmissor = ufEmissor;
        ArquivoPath = arquivoPath;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string tipo,
        string numero,
        DateOnly? dataEmissao = null,
        DateOnly? dataValidade = null,
        string? orgaoEmissor = null,
        string? ufEmissor = null,
        string? arquivoPath = null)
    {
        Tipo = tipo;
        Numero = numero;
        DataEmissao = dataEmissao;
        DataValidade = dataValidade;
        OrgaoEmissor = orgaoEmissor;
        UfEmissor = ufEmissor;
        ArquivoPath = arquivoPath;
        UpdatedAt = DateTime.UtcNow;
    }
}
