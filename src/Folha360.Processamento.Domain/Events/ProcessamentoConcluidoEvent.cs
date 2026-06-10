namespace Folha360.Processamento.Domain.Events;

public record ProcessamentoConcluidoEvent(
    Guid ProcessamentoId,
    Guid EmpresaId,
    string Periodo,
    TipoCalculo TipoCalculo,
    DateTime OcorridoEm);
