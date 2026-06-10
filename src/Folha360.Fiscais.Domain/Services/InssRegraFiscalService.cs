using System.Text.Json;
using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class InssRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var p = JsonSerializer.Deserialize<InssParametros>(parametros.ParametrosJson)
            ?? new InssParametros();

        var aliquotaPatronal = p.AliquotaPatronal + p.Rat + p.Fap + p.Terceiros;
        var partePatronal = contexto.BaseCalculoTotal * aliquotaPatronal;
        var valorTotal = contexto.ValorTotalFolha + partePatronal;

        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 20)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.INSS,
            contexto.BaseCalculoTotal,
            aliquotaPatronal,
            valorTotal,
            vencimento,
            parametros.CodigoReceita);
    }

    private class InssParametros
    {
        public decimal AliquotaPatronal { get; set; } = 0.20m;
        public decimal Rat { get; set; } = 0.02m;
        public decimal Fap { get; set; } = 0.01m;
        public decimal Terceiros { get; set; } = 0.058m;
    }
}
