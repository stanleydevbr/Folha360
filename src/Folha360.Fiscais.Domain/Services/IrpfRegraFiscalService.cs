using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class IrpfRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 20)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.IRRF,
            contexto.ValorTotalFolha,
            0,
            contexto.ValorTotalFolha,
            vencimento,
            parametros.CodigoReceita);
    }
}
