namespace Folha360.Processamento.Domain.Events;

public record FolhaReabertaEvent(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId,
    int Versao,
    string Motivo,
    string Autor,
    DateTime OcorridoEm);
