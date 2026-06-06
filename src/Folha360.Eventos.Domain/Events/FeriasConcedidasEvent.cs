namespace Folha360.Eventos.Domain.Events;

public record FeriasConcedidasEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid FeriasId,
    DateTime OcorridoEm)
{
    public FeriasConcedidasEvent(
        Guid funcionarioId,
        Guid empresaId,
        Guid feriasId)
        : this(funcionarioId, empresaId, feriasId, DateTime.UtcNow)
    {
    }
}
