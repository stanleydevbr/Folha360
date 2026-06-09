using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class SindicalRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 10)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.ContribuicaoSindical,
            contexto.ValorTotalFolha,
            0,
            contexto.ValorTotalFolha,
            vencimento,
            parametros.CodigoReceita);
    }
}
