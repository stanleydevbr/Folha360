namespace Folha360.Cadastros.Domain.Services;

/// <summary>
/// Resultado do cálculo de um funcionário, contendo valores por rubrica e totais.
/// </summary>
public class ResultadoCalculo
{
    public Guid FuncionarioId { get; init; }
    public Dictionary<Guid, decimal> ValoresPorRubrica { get; init; } = new();
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal Liquido { get; set; }
    public decimal BaseInss { get; set; }
    public decimal BaseIrrf { get; set; }
    public decimal BaseFgts { get; set; }
    public List<string> Erros { get; init; } = new();
}
