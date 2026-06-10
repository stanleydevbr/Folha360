namespace Folha360.Relatorios.Domain.Entities;

public class ItemFolhaView
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public Guid FuncionarioId { get; set; }
    public string NomeFuncionario { get; set; } = string.Empty;
    public Guid DepartamentoId { get; set; }
    public string NomeDepartamento { get; set; } = string.Empty;
    public Guid RubricaId { get; set; }
    public string CodigoRubrica { get; set; } = string.Empty;
    public string NomeRubrica { get; set; } = string.Empty;
    public string Natureza { get; set; } = string.Empty;
    public string TipoCalculo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
