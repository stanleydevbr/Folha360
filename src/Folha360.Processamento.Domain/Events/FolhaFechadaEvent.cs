namespace Folha360.Processamento.Domain.Events;

public record FolhaFechadaEvent(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId,
    decimal TotalVencimentos,
    decimal TotalDescontos,
    decimal TotalLiquido,
    int TotalFuncionarios,
    DateTime OcorridoEm);
