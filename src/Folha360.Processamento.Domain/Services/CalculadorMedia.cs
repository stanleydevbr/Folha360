using Folha360.Processamento.Domain.Abstractions;

namespace Folha360.Processamento.Domain.Services;

public interface ICalculadorMedia
{
    Task<decimal> CalcularMediaAsync(
        Guid funcionarioId,
        Guid rubricaId,
        int meses,
        DateOnly periodoReferencia,
        CancellationToken ct = default);
}

public class CalculadorMedia : ICalculadorMedia
{
    private readonly IItemFolhaRepository _itemFolhaRepository;

    public CalculadorMedia(IItemFolhaRepository itemFolhaRepository)
    {
        _itemFolhaRepository = itemFolhaRepository;
    }

    public async Task<decimal> CalcularMediaAsync(
        Guid funcionarioId,
        Guid rubricaId,
        int meses,
        DateOnly periodoReferencia,
        CancellationToken ct = default)
    {
        var historico = await _itemFolhaRepository.GetHistoricoMediasAsync(
            funcionarioId, rubricaId, meses, periodoReferencia, ct);

        var itens = historico.ToList();
        if (itens.Count == 0)
            return 0;

        return itens.Average(i => i.Valor);
    }
}
