using Folha360.Domain;

namespace Folha360.Fiscais.Domain.Entities;

public class GuiaRecolhimento : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public DateOnly Periodo { get; private set; }
    public Guid ApuracaoFiscalId { get; private set; }
    public TipoGuia TipoGuia { get; private set; }
    public Tributo Tributo { get; private set; }
    public string CodigoReceita { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateOnly DataVencimento { get; private set; }
    public string? MinioKey { get; private set; }
    public StatusGuia Status { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public decimal? ValorPago { get; private set; }
    public string? ComprovanteMinioKey { get; private set; }

    private GuiaRecolhimento()
    {
    }

    public GuiaRecolhimento(
        Guid empresaId,
        DateOnly periodo,
        Guid apuracaoFiscalId,
        TipoGuia tipoGuia,
        Tributo tributo,
        string codigoReceita,
        decimal valor,
        DateOnly dataVencimento)
    {
        EmpresaId = empresaId;
        Periodo = periodo;
        ApuracaoFiscalId = apuracaoFiscalId;
        TipoGuia = tipoGuia;
        Tributo = tributo;
        CodigoReceita = codigoReceita;
        Valor = valor;
        DataVencimento = dataVencimento;
        Status = StatusGuia.Pendente;
    }

    public void Gerar(string minioKey)
    {
        MinioKey = minioKey;
        Status = StatusGuia.Gerada;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegistrarPagamento(decimal valorPago, DateTime dataPagamento, string? comprovanteMinioKey = null)
    {
        ValorPago = valorPago;
        DataPagamento = dataPagamento;
        ComprovanteMinioKey = comprovanteMinioKey;
        Status = StatusGuia.Paga;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Status = StatusGuia.Cancelada;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Vencer()
    {
        if (Status == StatusGuia.Gerada)
        {
            Status = StatusGuia.Vencida;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
