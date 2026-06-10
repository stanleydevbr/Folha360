namespace Folha360.Fiscais.Domain.Abstractions;

public interface IRegraFiscalService
{
    ApuracaoResult Calcular(ApuracaoContext contexto, RegraFiscalParametros parametros);
}

public record ApuracaoContext(
    Guid EmpresaId,
    DateOnly Periodo,
    Guid ProcessamentoId,
    string RegimeTributario,
    string? Municipio,
    List<Guid> FuncionariosIds,
    decimal BaseCalculoTotal,
    decimal ValorTotalFolha);

public record RegraFiscalParametros(
    Tributo Tributo,
    string ParametrosJson,
    string CodigoReceita);

public record ApuracaoResult(
    Tributo Tributo,
    decimal BaseCalculo,
    decimal Aliquota,
    decimal ValorDevido,
    DateOnly DataVencimento,
    string CodigoReceita);
