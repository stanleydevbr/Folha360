namespace Folha360.Fiscais.Application.DTOs;

public record ApuracaoFiscalDto(
    Guid Id,
    Guid EmpresaId,
    string Periodo,
    string Tributo,
    decimal BaseCalculo,
    decimal Aliquota,
    decimal ValorDevido,
    string Status,
    string DataVencimento);

public record GuiaRecolhimentoDto(
    Guid Id,
    string TipoGuia,
    string Tributo,
    decimal Valor,
    string DataVencimento,
    string Status,
    string? DownloadUrl);

public record ResumoApuracaoDto(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId,
    string Status,
    Dictionary<string, decimal> TotaisPorTributo,
    List<GuiaRecolhimentoDto> Guias,
    DateTime DataApuracao);

public record StatusFiscalDto(
    string? UltimoPeriodo,
    int GuiasPendentes,
    int GuiasVencidas,
    List<DateTime> ProximosVencimentos);

public record RegraFiscalDto(
    Guid Id,
    string Tributo,
    int Versao,
    string VigenciaInicio,
    string? VigenciaFim,
    string CodigoReceita,
    bool Ativo);

public record ExportacaoDto(
    Guid Id,
    string Formato,
    string? DownloadUrl,
    DateTime DataGeracao);
