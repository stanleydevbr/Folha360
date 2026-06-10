namespace Folha360.Processamento.Application.DTOs;

public record ReabrirProcessamentoRequest(
    Guid ProcessamentoId,
    string Motivo,
    Guid Autor);

public record HistoricoProcessamentoResponse(
    Guid ProcessamentoId,
    int Versao,
    string Status,
    DateTime DataInicio,
    DateTime? DataFim,
    string? ReabertoPor,
    string? MotivoReabertura,
    DateTime? ReabertoEm);

public record ReaberturaStatusResponse(
    Guid ReaberturaId,
    string Estado,
    string EtapaAtual,
    DateTime DataInicio,
    string? Erro);
