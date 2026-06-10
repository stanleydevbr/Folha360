using System.Text.Json;
using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class PisRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var p = JsonSerializer.Deserialize<RegimeAliquota>(parametros.ParametrosJson)
            ?? new RegimeAliquota();

        var aliquota = contexto.RegimeTributario == "Lucro Real"
            ? p.AliquotaLucroReal
            : p.AliquotaPadrao;

        var valorDevido = contexto.BaseCalculoTotal * aliquota;
        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 25)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.PIS,
            contexto.BaseCalculoTotal,
            aliquota,
            valorDevido,
            vencimento,
            parametros.CodigoReceita);
    }

    private class RegimeAliquota
    {
        public decimal AliquotaLucroReal { get; set; } = 0.0065m;
        public decimal AliquotaPadrao { get; set; } = 0.01m;
    }
}
