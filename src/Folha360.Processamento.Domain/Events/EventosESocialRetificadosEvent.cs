namespace Folha360.Processamento.Domain.Events;

public record EventosESocialRetificadosEvent(
    Guid EmpresaId,
    string Periodo,
    DateTime OcorridoEm);
