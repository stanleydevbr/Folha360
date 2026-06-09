using Folha360.Domain;

namespace Folha360.Fiscais.Domain.Entities;

public class LancamentoContabil : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public DateOnly Periodo { get; private set; }
    public Guid ApuracaoFiscalId { get; private set; }
    public DateOnly Data { get; private set; }
    public string ContaDebito { get; private set; } = string.Empty;
    public string ContaCredito { get; private set; } = string.Empty;
    public string Historico { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public Tributo Tributo { get; private set; }
    public FormatoExportacao Formato { get; private set; }
    public string? MinioKey { get; private set; }

    private LancamentoContabil()
    {
    }

    public LancamentoContabil(
        Guid empresaId,
        DateOnly periodo,
        Guid apuracaoFiscalId,
        DateOnly data,
        string contaDebito,
        string contaCredito,
        string historico,
        decimal valor,
        Tributo tributo,
        FormatoExportacao formato)
    {
        EmpresaId = empresaId;
        Periodo = periodo;
        ApuracaoFiscalId = apuracaoFiscalId;
        Data = data;
        ContaDebito = contaDebito;
        ContaCredito = contaCredito;
        Historico = historico;
        Valor = valor;
        Tributo = tributo;
        Formato = formato;
    }

    public void DefinirMinioKey(string minioKey)
    {
        MinioKey = minioKey;
        UpdatedAt = DateTime.UtcNow;
    }
}
