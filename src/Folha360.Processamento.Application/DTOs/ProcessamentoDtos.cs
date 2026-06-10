namespace Folha360.Processamento.Application.DTOs;

public record IniciarProcessamentoRequest(
    Guid EmpresaId,
    string Periodo,
    string TipoCalculo);

public record ProcessamentoResponse(
    Guid Id,
    Guid EmpresaId,
    string Periodo,
    string TipoCalculo,
    string Status,
    int Versao,
    int TotalFuncionarios,
    int FuncionariosProcessados,
    int FuncionariosComErro,
    decimal TotalVencimentos,
    decimal TotalDescontos,
    decimal TotalLiquido,
    DateTime? DataInicio,
    DateTime? DataFim,
    string? Erro);

public record ItemFolhaResponse(
    Guid Id,
    Guid RubricaId,
    string CodigoRubrica,
    string DescricaoRubrica,
    string Fase,
    decimal BaseCalculo,
    decimal Valor,
    string? FormulaAplicada,
    int Ordem);
