namespace Folha360.Fiscais.Domain.Events;

public record GuiaEmitidaEvent(
    Guid EmpresaId,
    string Periodo,
    Guid GuiaId,
    TipoGuia TipoGuia,
    Tributo Tributo,
    decimal Valor,
    DateTime Vencimento,
    DateTime OcorridoEm);
