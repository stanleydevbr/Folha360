namespace Folha360.Processamento.Domain.Events;

public record RetificarEventosESocialCommand(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId);
