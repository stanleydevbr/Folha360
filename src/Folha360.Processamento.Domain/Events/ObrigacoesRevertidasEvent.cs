namespace Folha360.Processamento.Domain.Events;

public record ObrigacoesRevertidasEvent(
    Guid EmpresaId,
    string Periodo,
    DateTime OcorridoEm);
