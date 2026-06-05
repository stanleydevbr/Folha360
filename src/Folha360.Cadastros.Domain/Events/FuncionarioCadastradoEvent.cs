namespace Folha360.Cadastros.Domain.Events;

public record FuncionarioCadastradoEvent(
    Guid FuncionarioId,
    Guid EmpresaId,
    Guid CargoId,
    decimal SalarioBase,
    DateOnly DataAdmissao,
    DateTime OcorridoEm)
{
    public FuncionarioCadastradoEvent(
        Guid funcionarioId,
        Guid empresaId,
        Guid cargoId,
        decimal salarioBase,
        DateOnly dataAdmissao)
        : this(funcionarioId, empresaId, cargoId, salarioBase, dataAdmissao, DateTime.UtcNow)
    {
    }
}
