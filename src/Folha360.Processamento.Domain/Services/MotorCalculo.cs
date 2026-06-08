using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Processamento.Domain.Services;

public interface IMotorCalculo
{
    ResultadoCalculo Processar(
        Guid funcionarioId,
        IReadOnlyList<Rubrica> rubricas,
        IReadOnlyDictionary<string, object> contextoFuncionario,
        TipoCalculo tipoCalculo,
        DateOnly periodo,
        CancellationToken ct = default);
}

public class MotorCalculo : IMotorCalculo
{
    private readonly IAvaliadorExpressao _avaliadorExpressao;
    private readonly IResolvedorComposicao _resolvedorComposicao;
    private readonly IAplicadorTabelaProgressiva _aplicadorTabela;
    private readonly IAvaliadorCondicional _avaliadorCondicional;

    public MotorCalculo(
        IAvaliadorExpressao avaliadorExpressao,
        IResolvedorComposicao resolvedorComposicao,
        IAplicadorTabelaProgressiva aplicadorTabela,
        IAvaliadorCondicional avaliadorCondicional)
    {
        _avaliadorExpressao = avaliadorExpressao;
        _resolvedorComposicao = resolvedorComposicao;
        _aplicadorTabela = aplicadorTabela;
        _avaliadorCondicional = avaliadorCondicional;
    }

    public ResultadoCalculo Processar(
        Guid funcionarioId,
        IReadOnlyList<Rubrica> rubricas,
        IReadOnlyDictionary<string, object> contextoFuncionario,
        TipoCalculo tipoCalculo,
        DateOnly periodo,
        CancellationToken ct = default)
    {
        var resultado = new ResultadoCalculo { FuncionarioId = funcionarioId };
        var valoresCalculados = new Dictionary<Guid, decimal>();

        var ordenadas = rubricas.OrderBy(r => r.OrdemCalculo).ToList();

        // Fase 1: Vencimentos (ordem_calculo 1-99)
        foreach (var rubrica in ordenadas.Where(r => r.OrdemCalculo < 100 && r.Ativo))
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados);
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
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados);
                valoresCalculados[rubrica.Id] = valor;
                resultado.ValoresPorRubrica[rubrica.Id] = valor;

                if (rubrica.Codigo == "BASE-INSS")
                    resultado.BaseInss = valor;
                if (rubrica.Codigo == "BASE-IRRF")
                    resultado.BaseIrrf = valor;
                if (rubrica.Codigo == "BASE-FGTS")
                    resultado.BaseFgts = valor;
            }
            catch (Exception ex)
            {
                resultado.Erros.Add($"Erro na rubrica {rubrica.Codigo}: {ex.Message}");
            }
        }

        // Fase 3: Descontos (ordem_calculo 200-299)
        ct.ThrowIfCancellationRequested();
        var descontos = ordenadas
            .Where(r => r.OrdemCalculo >= 200 && r.OrdemCalculo < 300 && r.Ativo)
            .OrderBy(r => r.PrioridadeDesconto ?? 999)
            .ThenBy(r => r.OrdemCalculo);

        foreach (var rubrica in descontos)
        {
            try
            {
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados);
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
                var valor = CalcularRubrica(rubrica, contextoFuncionario, valoresCalculados);
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
        Dictionary<Guid, decimal> valoresCalculados)
    {
        var valor = rubrica.TipoCalculo switch
        {
            "VALOR_FIXO" => rubrica.ValorFixo ?? 0,
            "PERCENTUAL" => CalcularPercentual(rubrica, valoresCalculados),
            "HORA" => CalcularHora(contexto),
            "DIA" => CalcularDia(contexto),
            "UNIDADE" => CalcularUnidade(contexto),
            "FORMULA" => CalcularFormula(rubrica, contexto, valoresCalculados),
            "COMPOSICAO" => _resolvedorComposicao.Resolver(rubrica, valoresCalculados),
            "TABELA_PROGRESSIVA" => _aplicadorTabela.Aplicar(
                ObterBase(rubrica, valoresCalculados),
                rubrica.TabelasProgressivas.ToList()),
            "TETO" => CalcularTeto(rubrica, valoresCalculados),
            "CONDICIONAL" => CalcularCondicional(rubrica, contexto, valoresCalculados),
            _ => 0,
        };

        if (rubrica.TetoMaximo.HasValue && valor > rubrica.TetoMaximo.Value)
            valor = rubrica.TetoMaximo.Value;
        if (rubrica.PisoMinimo.HasValue && valor < rubrica.PisoMinimo.Value)
            valor = rubrica.PisoMinimo.Value;

        return valor;
    }

    private static decimal CalcularPercentual(Rubrica rubrica, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (!rubrica.RubricaBaseId.HasValue || !rubrica.Percentual.HasValue)
            return 0;

        var baseValor = valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v) ? v : 0;
        return baseValor * rubrica.Percentual.Value / 100;
    }

    private static decimal CalcularHora(IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade_horas");
        var valorHora = ObterContexto<decimal>(contexto, "valor_hora");
        return quantidade * valorHora;
    }

    private static decimal CalcularDia(IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade_dias");
        var valorDia = ObterContexto<decimal>(contexto, "valor_dia");
        return quantidade * valorDia;
    }

    private static decimal CalcularUnidade(IReadOnlyDictionary<string, object> contexto)
    {
        var quantidade = ObterContexto<decimal>(contexto, "quantidade");
        var valorUnitario = ObterContexto<decimal>(contexto, "valor_unitario");
        return quantidade * valorUnitario;
    }

    private decimal CalcularFormula(
        Rubrica rubrica,
        IReadOnlyDictionary<string, object> contexto,
        Dictionary<Guid, decimal> valoresCalculados)
    {
        var formulaTexto = rubrica.Formula?.Expressao ?? rubrica.FormulaCalculo;
        if (string.IsNullOrWhiteSpace(formulaTexto))
            return 0;

        var variaveis = new Dictionary<string, decimal>();
        foreach (var kv in valoresCalculados)
        {
            variaveis[$"VALOR_RUBRICA_{kv.Key}"] = kv.Value;
        }

        foreach (var kv in contexto)
        {
            if (kv.Value is decimal d)
                variaveis[kv.Key] = d;
            else if (kv.Value is int i)
                variaveis[kv.Key] = i;
            else if (kv.Value is double db)
                variaveis[kv.Key] = (decimal)db;
            else if (kv.Value is long l)
                variaveis[kv.Key] = l;
        }

        return _avaliadorExpressao.Avaliar(formulaTexto, variaveis);
    }

    private static decimal CalcularTeto(Rubrica rubrica, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (!rubrica.RubricaBaseId.HasValue)
            return 0;

        var baseValor = valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v) ? v : 0;

        if (rubrica.TetoMaximo.HasValue && baseValor > rubrica.TetoMaximo.Value)
            return rubrica.TetoMaximo.Value;

        return baseValor;
    }

    private decimal CalcularCondicional(
        Rubrica rubrica,
        IReadOnlyDictionary<string, object> contexto,
        Dictionary<Guid, decimal> valoresCalculados)
    {
        var condicao = rubrica.Formula?.Expressao ?? rubrica.FormulaCalculo;
        if (string.IsNullOrWhiteSpace(condicao))
            return 0;

        var condicaoContexto = new Dictionary<string, object>(contexto);
        foreach (var kv in valoresCalculados)
        {
            condicaoContexto[$"VALOR_RUBRICA_{kv.Key}"] = kv.Value;
        }

        if (!_avaliadorCondicional.Avaliar(condicao, condicaoContexto))
            return 0;

        if (rubrica.ValorFixo.HasValue)
            return rubrica.ValorFixo.Value;

        if (rubrica.Percentual.HasValue && rubrica.RubricaBaseId.HasValue)
            return CalcularPercentual(rubrica, valoresCalculados);

        return 0;
    }

    private static decimal ObterBase(Rubrica rubrica, Dictionary<Guid, decimal> valoresCalculados)
    {
        if (rubrica.RubricaBaseId.HasValue && valoresCalculados.TryGetValue(rubrica.RubricaBaseId.Value, out var v))
            return v;

        return 0;
    }

    private static T ObterContexto<T>(IReadOnlyDictionary<string, object> contexto, string chave)
    {
        if (contexto.TryGetValue(chave, out var valor))
        {
            if (valor is T t)
                return t;
            if (valor is IConvertible)
                return (T)Convert.ChangeType(valor, typeof(T));
        }

        return default!;
    }
}
