namespace Folha360.Fiscais.Domain.Events;

public record ObrigacoesRevertidasEvent(
    Guid EmpresaId,
    string Periodo,
    DateTime OcorridoEm);
