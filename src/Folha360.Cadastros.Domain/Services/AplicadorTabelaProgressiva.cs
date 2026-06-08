using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Aplicador de tabelas progressivas (IRRF/INSS).
/// Itera faixas em ordem e aplica alíquota com dedução.
/// </summary>
public class AplicadorTabelaProgressiva
{
    /// <summary>
    /// Aplica a tabela progressiva sobre a base de cálculo.
    /// Fórmula: (baseCalculo * aliquota) - deducao para a última faixa aplicável.
    /// </summary>
    public decimal Aplicar(decimal baseCalculo, IReadOnlyList<RubricaTabelaProgressiva> faixas)
    {
        if (faixas.Count == 0)
        {
            return 0;
        }

        var faixasOrdenadas = faixas.OrderBy(f => f.Ordem).ToList();
        decimal imposto = 0;
        decimal baseRestante = baseCalculo;

        for (int i = 0; i < faixasOrdenadas.Count; i++)
        {
            var faixa = faixasOrdenadas[i];
            var limiteSuperior = faixa.FaixaAte ?? decimal.MaxValue;
            var valorNaFaixa = Math.Min(baseRestante, limiteSuperior - faixa.FaixaDe);

            if (valorNaFaixa > 0)
            {
                imposto += valorNaFaixa * faixa.Aliquota / 100;
            }

            baseRestante -= valorNaFaixa;
            if (baseRestante <= 0)
            {
                break;
            }
        }

        return imposto;
    }
}
