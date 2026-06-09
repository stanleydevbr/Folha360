namespace Folha360.Fiscais.Domain.Exceptions;

public class RegraFiscalNaoEncontradaException : Exception
{
    public Tributo Tributo { get; }
    public DateOnly Periodo { get; }

    public RegraFiscalNaoEncontradaException(Tributo tributo, DateOnly periodo)
        : base($"Regra fiscal não encontrada para o tributo {tributo} no período {periodo:yyyy-MM}.")
    {
        Tributo = tributo;
        Periodo = periodo;
    }
}
