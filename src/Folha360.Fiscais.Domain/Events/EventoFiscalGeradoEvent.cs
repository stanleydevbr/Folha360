namespace Folha360.Fiscais.Domain.Events;

public record EventoFiscalGeradoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid ProcessamentoId,
    string Periodo,
    Tributo Tributo,
    decimal BaseCalculo,
    decimal Valor,
    DateTime OcorridoEm);
