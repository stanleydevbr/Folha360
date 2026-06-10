using System.Text.Json;
using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class IssRegraFiscalService : IRegraFiscalService
{
    public ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros)
    {
        var p = JsonSerializer.Deserialize<IssParametros>(parametros.ParametrosJson)
            ?? new IssParametros();

        var aliquota = p.AliquotasPorMunicipio
            ?.FirstOrDefault(a => a.Municipio?.Equals(contexto.Municipio, StringComparison.OrdinalIgnoreCase) == true)
            ?.Aliquota
            ?? p.AliquotaPadrao;

        var valorDevido = contexto.BaseCalculoTotal * aliquota;
        var vencimento = new DateOnly(contexto.Periodo.Year, contexto.Periodo.Month, 15)
            .AddMonths(1);

        return new ApuracaoResult(
            Tributo.ISS,
            contexto.BaseCalculoTotal,
            aliquota,
            valorDevido,
            vencimento,
            parametros.CodigoReceita);
    }

    private class IssParametros
    {
        public decimal AliquotaPadrao { get; set; } = 0.05m;
        public List<MunicipioAliquota>? AliquotasPorMunicipio { get; set; }

        public class MunicipioAliquota
        {
            public string? Municipio { get; set; }
            public decimal Aliquota { get; set; }
        }
    }
}
