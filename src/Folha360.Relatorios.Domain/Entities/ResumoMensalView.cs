namespace Folha360.Relatorios.Domain.Entities;

public class ResumoMensalView
{
    public int TotalFuncionarios { get; set; }
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal TotalLiquido { get; set; }
    public decimal TotalIrrf { get; set; }
    public decimal TotalInss { get; set; }
    public decimal TotalFgts { get; set; }
}
