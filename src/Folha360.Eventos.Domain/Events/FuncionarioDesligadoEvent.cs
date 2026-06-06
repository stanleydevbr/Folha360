namespace Folha360.Eventos.Domain.Events;

public record FuncionarioDesligadoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid DesligamentoId,
    DateTime OcorridoEm)
{
    public FuncionarioDesligadoEvent(
        Guid funcionarioId,
        Guid empresaId,
        Guid desligamentoId)
        : this(funcionarioId, empresaId, desligamentoId, DateTime.UtcNow)
    {
    }
}
