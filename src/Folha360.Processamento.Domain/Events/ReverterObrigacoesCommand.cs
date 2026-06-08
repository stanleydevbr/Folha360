namespace Folha360.Processamento.Domain.Events;

public record ReverterObrigacoesCommand(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId);
