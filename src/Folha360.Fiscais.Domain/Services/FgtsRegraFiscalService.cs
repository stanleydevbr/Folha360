using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class FgtsRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var valorDevido = contexto.BaseCalculoTotal * 0.08m;
        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 7)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.FGTS,
            contexto.BaseCalculoTotal,
            0.08m,
            valorDevido,
            vencimento,
            parametros.CodigoReceita);
    }
}
