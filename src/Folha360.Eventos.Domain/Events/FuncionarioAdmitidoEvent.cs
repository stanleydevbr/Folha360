namespace Folha360.Eventos.Domain.Events;

public record FuncionarioAdmitidoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid AdmissaoId,
    DateTime OcorridoEm)
{
    public FuncionarioAdmitidoEvent(
        Guid funcionarioId,
        Guid empresaId,
        Guid admissaoId)
        : this(funcionarioId, empresaId, admissaoId, DateTime.UtcNow)
    {
    }
}
