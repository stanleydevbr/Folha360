namespace Folha360.Fiscais.Domain.Events;

public record ObrigacoesApuradasEvent(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId,
    Dictionary<Tributo, decimal> TotaisPorTributo,
    DateTime OcorridoEm);
