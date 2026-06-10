namespace Folha360.Processamento.Domain.Events;

public record EventoRemuneracaoGeradoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid ProcessamentoId,
    string Periodo,
    List<ItemRemuneracaoDto> Itens,
    DateTime OcorridoEm);
