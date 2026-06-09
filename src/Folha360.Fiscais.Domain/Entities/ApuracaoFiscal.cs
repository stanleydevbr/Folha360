using Folha360.Domain;

namespace Folha360.Fiscais.Domain.Entities;

public class ApuracaoFiscal : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public DateOnly Periodo { get; private set; }
    public Guid ProcessamentoId { get; private set; }
    public Tributo Tributo { get; private set; }
    public decimal BaseCalculo { get; private set; }
    public decimal Aliquota { get; private set; }
    public decimal ValorDevido { get; private set; }
    public decimal ValorPago { get; private set; }
    public DateOnly DataVencimento { get; private set; }
    public Guid? RegraFiscalId { get; private set; }
    public StatusApuracao Status { get; private set; }

    private ApuracaoFiscal()
    {
    }

    public ApuracaoFiscal(
        Guid empresaId,
        DateOnly periodo,
        Guid processamentoId,
        Tributo tributo)
    {
        EmpresaId = empresaId;
        Periodo = periodo;
        ProcessamentoId = processamentoId;
        Tributo = tributo;
        Status = StatusApuracao.Pendente;
    }

    public void Iniciar()
    {
        Status = StatusApuracao.EmProcessamento;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Concluir(decimal baseCalculo, decimal aliquota, decimal valorDevido, DateOnly dataVencimento, Guid regraFiscalId)
    {
        BaseCalculo = baseCalculo;
        Aliquota = aliquota;
        ValorDevido = valorDevido;
        DataVencimento = dataVencimento;
        RegraFiscalId = regraFiscalId;
        Status = StatusApuracao.Concluido;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Falhar()
    {
        Status = StatusApuracao.Falho;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reverter()
    {
        Status = StatusApuracao.Revertido;
        UpdatedAt = DateTime.UtcNow;
    }
}
