namespace Folha360.Processamento.Domain.Events;

public record ReverterObrigacoesEvent(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId);
