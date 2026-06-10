namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Calculador de médias móveis para rubricas do tipo MEDIA.
/// Utilizado para férias, 13º, rescisão (médias de horas extras, comissões, etc.).
/// </summary>
public class CalculadorMedia
{
    /// <summary>
    /// Calcula a média aritmética dos valores históricos para uma rubrica.
    /// </summary>
    public decimal Calcular(IReadOnlyDictionary<Guid, decimal> historicoMedias, Guid rubricaId)
    {
        if (historicoMedias.TryGetValue(rubricaId, out var media))
            return media;
        return 0;
    }
}
