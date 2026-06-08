using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Aplicador de tabelas progressivas (IRRF/INSS).
/// Suporta dois métodos: simplificado (RFB) e progressivo por faixa.
/// </summary>
public class AplicadorTabelaProgressiva
{
    /// <summary>
    /// Aplica a tabela progressiva usando o método simplificado da Receita Federal:
    /// (baseCalculo * aliquotaDaUltimaFaixa) - deducao.
    /// </summary>
    public decimal Aplicar(decimal baseCalculo, IReadOnlyList<RubricaTabelaProgressiva> faixas)
    {
        if (faixas.Count == 0)
        {
            return 0;
        }

        var faixasOrdenadas = faixas.OrderBy(f => f.Ordem).ToList();

        // Encontrar a última faixa aplicável
        RubricaTabelaProgressiva? faixaAplicavel = null;
        foreach (var faixa in faixasOrdenadas)
        {
            if (baseCalculo > faixa.FaixaDe)
            {
                faixaAplicavel = faixa;
            }
        }

        if (faixaAplicavel == null || faixaAplicavel.Aliquota == 0)
        {
            return 0;
        }

        return (baseCalculo * faixaAplicavel.Aliquota / 100) - faixaAplicavel.Deducao;
    }

    /// <summary>
    /// Aplica a tabela progressiva usando cálculo progressivo por faixa
    /// (soma dos valores de cada faixa). Usado para INSS.
    /// </summary>
    public decimal AplicarProgressivo(decimal baseCalculo, IReadOnlyList<RubricaTabelaProgressiva> faixas)
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
