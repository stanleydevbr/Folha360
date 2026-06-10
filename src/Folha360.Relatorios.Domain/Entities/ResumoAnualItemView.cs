namespace Folha360.Relatorios.Domain.Entities;

public class ResumoAnualItemView
{
    public string Periodo { get; set; } = string.Empty;
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal TotalLiquido { get; set; }
    public int TotalFuncionarios { get; set; }
}
