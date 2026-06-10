using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Processamento.Domain.Services;

public interface IResolvedorComposicao
{
    decimal Resolver(Rubrica rubrica, IReadOnlyDictionary<Guid, decimal> valoresCalculados);
    bool IsCicloSafe(Guid rubricaPrincipalId, Guid rubricaComponenteId, IReadOnlyList<RubricaComposicao> composicoesExistentes);
}

public class ResolvedorComposicao : IResolvedorComposicao
{
    public decimal Resolver(Rubrica rubrica, IReadOnlyDictionary<Guid, decimal> valoresCalculados)
    {
        var composicoes = rubrica.Composicoes.OrderBy(c => c.Ordem).ToList();
        decimal total = 0;

        foreach (var comp in composicoes)
        {
            if (!valoresCalculados.TryGetValue(comp.RubricaComponenteId, out var valorComponente))
            {
                if (comp.Obrigatorio)
                {
                    throw new InvalidOperationException(
                        $"Componente obrigatório {comp.RubricaComponenteId} não calculado para rubrica {rubrica.Codigo}.");
                }
                continue;
            }

            if (comp.PercentualComposicao.HasValue)
                valorComponente = valorComponente * comp.PercentualComposicao.Value / 100;

            total = comp.Operador switch
            {
                "+" => total + valorComponente,
                "-" => total - valorComponente,
                "*" => total * valorComponente,
                "/" => valorComponente != 0
                    ? total / valorComponente
                    : throw new DivideByZeroException($"Divisão por zero na composição da rubrica {rubrica.Codigo}"),
                _ => total + valorComponente,
            };
        }

        return total;
    }

    public bool IsCicloSafe(
        Guid rubricaPrincipalId,
        Guid rubricaComponenteId,
        IReadOnlyList<RubricaComposicao> composicoesExistentes)
    {
        var grafo = new Dictionary<Guid, List<Guid>>();
        foreach (var c in composicoesExistentes)
        {
            if (!grafo.ContainsKey(c.RubricaPrincipalId))
                grafo[c.RubricaPrincipalId] = new List<Guid>();
            grafo[c.RubricaPrincipalId].Add(c.RubricaComponenteId);
        }

        if (!grafo.ContainsKey(rubricaPrincipalId))
            grafo[rubricaPrincipalId] = new List<Guid>();
        grafo[rubricaPrincipalId].Add(rubricaComponenteId);

        return !TemCaminho(grafo, rubricaComponenteId, rubricaPrincipalId, new HashSet<Guid>());
    }

    private static bool TemCaminho(
        Dictionary<Guid, List<Guid>> grafo,
        Guid origem,
        Guid destino,
        HashSet<Guid> visitados)
    {
        if (origem == destino)
            return true;

        if (!grafo.ContainsKey(origem) || !visitados.Add(origem))
            return false;

        return grafo[origem].Any(vizinho => TemCaminho(grafo, vizinho, destino, visitados));
    }
}
