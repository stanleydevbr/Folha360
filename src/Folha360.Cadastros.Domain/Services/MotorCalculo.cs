using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Motor de cálculo da folha de pagamento. Processa rubricas em 4 fases:
/// Vencimentos → Bases → Descontos → Totais.
/// </summary>
public class MotorCalculo
{
    private readonly IExpressionEvaluator _avaliador;
    private readonly ResolvedorComposicao _resolvedorComposicao;
    private readonly AplicadorTabelaProgressiva _aplicadorTabela;
    private readonly CalculadorMedia _calculadorMedia;
    private readonly AvaliadorCondicional _avaliadorCondicional;

    public MotorCalculo(
        IExpressionEvaluator avaliador,
        ResolvedorComposicao resolvedorComposicao,
        AplicadorTabelaProgressiva aplicadorTabela,
        CalculadorMedia calculadorMedia,
        AvaliadorCondicional avaliadorCondicional)
    {
        _avaliador = avaliador;
        _resolvedorComposicao = resolvedorComposicao;
        _aplicadorTabela = aplicadorTabela;
        _calculadorMedia = calculadorMedia;
        _avaliadorCondicional = avaliadorCondicional;
    }

    public ResultadoCalculo Calcular(
        IReadOnlyList<Rubrica> rubricas,
        IReadOnlyDictionary<string, object> contextoFuncionario,
        IReadOnlyDictionary<Guid, decimal>? historicoMedias = null,
        CancellationToken ct = default)
    {
        var resultado = new ResultadoCalculo();
        var valoresCalculados = new Dictionary<Guid, decimal>();

        // Ordenar rubricas por ordem_calculo
        var ordenadas = rubricas.OrderBy(r => r.OrdemCalculo).ToList();

        // Fase 1: Vencimentos (ordem_calculo 1-99)
        foreach (var rubrica in ordenadas.Where(r => r.OrdemCalculo < 100 && r.Ativo))
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados, historicoMedias);
                valoresCalculados[rubrica.Id] = valor;
                resultado.ValoresPorRubrica[rubrica.Id] = valor;
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro na rubrica {rubrica.Codigo}: {ex.Message}");
            }
        }

        resultado.TotalVencimentos = valoresCalculados
            .Where(kv =>
            {
                var rubrica = ordenadas.FirstOrDefault(r => r.Id == kv.Key);
                return rubrica != null && rubrica.Natureza != "Desconto";
            })
            .Sum(kv => kv.Value);

        // Fase 2: Bases (ordem_calculo 100-199)
        ct.ThrowIfCancellationRequested();
        foreach (var rubrica in ordenadas.Where(r => r.OrdemCalculo >= 100 && r.OrdemCalculo < 200 && r.Ativo))
        {
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados, historicoMedias);
                valoresCalculados[rubrica.Id] = valor;
                resultado.ValoresPorRubrica[rubrica.Id] = valor;

                if (rubrica.Codigo == "BASE-INSS")
                {
                    resultado.BaseInss = valor;
                }

                if (rubrica.Codigo == "BASE-IRRF")
                {
                    resultado.BaseIrrf = valor;
                }

                if (rubrica.Codigo == "BASE-FGTS")
                {
                    resultado.BaseFgts = valor;
                }
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro na rubrica {rubrica.Codigo}: {ex.Message}");
            }
        }

        // Fase 3: Descontos (ordem_calculo 200-299)
        ct.ThrowIfCancellationRequested();

        // Ordenar por prioridade_desconto dentro da fase
        var descontos = ordenadas
            .Where(r => r.OrdemCalculo >= 200 && r.OrdemCalculo < 300 && r.Ativo)
            .OrderBy(r => r.PrioridadeDesconto ?? 999)
            .ThenBy(r => r.OrdemCalculo);

        foreach (var rubrica in descontos)
        {
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados, historicoMedias);
                valoresCalculados[rubrica.Id] = valor;
                resultado.ValoresPorRubrica[rubrica.Id] = valor;
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro na rubrica {rubrica.Codigo}: {ex.Message}");
            }
        }

        resultado.TotalDescontos = descontos.Sum(r =>
            valoresCalculados.TryGetValue(r.Id, out var v) ? v : 0);

        // Fase 4: Totais (ordem_calculo 300-399)
        ct.ThrowIfCancellationRequested();
        foreach (var rubrica in ordenadas.Where(r => r.OrdemCalculo >= 300 && r.Ativo))
        {
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados, historicoMedias);
                valoresCalculados[rubrica.Id] = valor;
                resultado.ValoresPorRubrica[rubrica.Id] = valor;
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro na rubrica {rubrica.Codigo}: {ex.Message}");
            }
        }

        resultado.Liquido = resultado.TotalVencimentos - resultado.TotalDescontos;
        return resultado;
    }

    private decimal CalcularRubrica(
        Rubrica rubrica,
        IReadOnlyDictionary<string, object> contexto,
        Dictionary<Guid, decimal> valoresCalculados,
        IReadOnlyDictionary<Guid, decimal>? historicoMedias)
    {
        var valor = rubrica.TipoCalculo switch
        {
            "VALOR_FIXO" => rubrica.ValorFixo ?? 0,
            "PERCENTUAL" => CalcularPercentual(rubrica, contexto, valoresCalculados),
            "HORA" => CalcularHora(rubrica, contexto),
            "DIA" => CalcularDia(rubrica, contexto),
            "UNIDADE" => CalcularUnidade(rubrica, contexto),
            "FORMULA" => CalcularFormula(rubrica, contexto, valoresCalculados),
            "COMPOSICAO" => _resolvedorComposicao.Resolver(rubrica, valoresCalculados),
            "TABELA_PROGRESSIVA" => _aplicadorTabela.Aplicar(ObterBase(rubrica, valoresCalculados), rubrica.TabelasProgressivas.ToList()),
            "MEDIA" => _calculadorMedia.Calcular(historicoMedias ?? new Dictionary<Guid, decimal>(), rubrica.Id),
            "TETO" => CalcularTeto(rubrica, contexto, valoresCalculados),
            "CONDICIONAL" => CalcularCondicional(rubrica, contexto, valoresCalculados),
            _ => 0
        };

        // Aplicar limites
        if (rubrica.TetoMaximo.HasValue && valor > rubrica.TetoMaximo.Value)
            valor = rubrica.TetoMaximo.Value;
        if (rubrica.PisoMinimo.HasValue && valor < rubrica.PisoMinimo.Value)
            valor = rubrica.PisoMinimo.Value;

        return valor;
    }

    private decimal CalcularPercentual(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (!rubrica.RubricaBaseId.HasValue || !rubrica.Percentual.HasValue)
            return 0;

        var baseValor = valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v) ? v : 0;
        return baseValor * rubrica.Percentual.Value / 100;
    }

    private decimal CalcularHora(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade_horas");
        var valorHora = ObterContexto<decimal>(contexto, "valor_hora");
        return quantidade * valorHora;
    }

    private decimal CalcularDia(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade_dias");
        var valorDia = ObterContexto<decimal>(contexto, "valor_dia");
        return quantidade * valorDia;
    }

    private decimal CalcularUnidade(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade");
        var valorUnitario = ObterContexto<decimal>(contexto, "valor_unitario");
        return quantidade * valorUnitario;
    }

    private decimal CalcularFormula(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (string.IsNullOrWhiteSpace(rubrica.FormulaCalculo))
            return 0;

        var parametros = new Dictionary<string, object>(contexto);
        foreach (var kv in valoresCalculados)
            parametros[$"RUBRICA_{kv.Key}"] = kv.Value;

        return _avaliador.Avaliar(rubrica.FormulaCalculo, parametros);
    }

    private decimal CalcularTeto(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (!rubrica.RubricaBaseId.HasValue)
            return 0;

        var baseValor = valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v) ? v : 0;
        return rubrica.TetoMaximo.HasValue && baseValor > rubrica.TetoMaximo.Value
            ? rubrica.TetoMaximo.Value
            : baseValor;
    }

    private decimal CalcularCondicional(Rubrica rubrica, IReadOnlyDictionary<string, object> contexto, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (string.IsNullOrWhiteSpace(rubrica.FormulaCalculo))
            return 0;

        var parametros = new Dictionary<string, object>(contexto);
        foreach (var kv in valoresCalculados)
            parametros[$"RUBRICA_{kv.Key}"] = kv.Value;

        var condicaoVerdadeira = _avaliadorCondicional.Avaliar(rubrica.FormulaCalculo, parametros);
        if (!condicaoVerdadeira)
            return 0;

        return rubrica.ValorFixo ?? CalcularPercentual(rubrica, contexto, valoresCalculados);
    }

    private decimal ObterBase(Rubrica rubrica, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (rubrica.RubricaBaseId.HasValue && valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v))
            return v;
        return 0;
    }

    private static T ObterContexto<T>(IReadOnlyDictionary<string, object> contexto, string chave)
    {
        if (contexto.TryGetValue(chave, out var valor) && valor is T typed)
            return typed;
        return default!;
    }
}
