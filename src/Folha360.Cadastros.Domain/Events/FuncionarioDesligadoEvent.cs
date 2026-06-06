namespace Folha360.Cadastros.Domain.Events;

public record FuncionarioDesligadoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    DateOnly DataDesligamento,
    string? Motivo,
    DateTime OcorridoEm)
{
    public FuncionarioDesligadoEvent(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataDesligamento,
        string? motivo = null)
        : this(funcionarioId, empresaId, dataDesligamento, motivo, DateTime.UtcNow)
    {
    }
}
