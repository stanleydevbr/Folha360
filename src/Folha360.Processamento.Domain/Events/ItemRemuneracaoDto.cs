namespace Folha360.Processamento.Domain.Events;

public record ItemRemuneracaoDto(
    Guid RubricaId,
    string Codigo,
    string Descricao,
    FaseProcessamento Fase,
    decimal BaseCalculo,
    decimal Valor,
    string? FormulaAplicada);
