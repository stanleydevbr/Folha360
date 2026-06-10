namespace Folha360.Relatorios.Domain.Events;

public class RelatoriosAtualizadosEvent
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<string> TiposRelatorio { get; set; } = new();
    public Guid CorrelationId { get; set; }
    public Guid CausationId { get; set; }
}
