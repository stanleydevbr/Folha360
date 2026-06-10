namespace Folha360.Relatorios.Domain.Entities;

public class FolhaSinteticaItemView
{
    public string CodigoRubrica { get; set; } = string.Empty;
    public string NomeRubrica { get; set; } = string.Empty;
    public string Natureza { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
