namespace Folha360.Processamento.Application.DTOs;

public record HoleriteResponse(
    Guid FuncionarioId,
    string NomeFuncionario,
    string MinioKey,
    DateTime DataGeracao,
    string DownloadUrl);
