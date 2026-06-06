using Folha360.Domain;

namespace Folha360.Eventos.Domain.Entities;

/// <summary>
/// Entidade Desligamento — representa o evento de desligamento de um funcionário.
/// Schema: tenant.
/// </summary>
public class Desligamento : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public DateOnly DataDesligamento { get; private set; }
    public MotivoDesligamento MotivoDesligamento { get; private set; }
    public string? VerbasRescisorias { get; private set; }
    public string? XmlContent { get; set; }

    private Desligamento()
    {
    }

    public Desligamento(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataDesligamento,
        MotivoDesligamento motivoDesligamento,
        string? verbasRescisorias = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataDesligamento = dataDesligamento;
        MotivoDesligamento = motivoDesligamento;
        VerbasRescisorias = verbasRescisorias;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly dataDesligamento,
        MotivoDesligamento motivoDesligamento,
        string? verbasRescisorias = null)
    {
        DataDesligamento = dataDesligamento;
        MotivoDesligamento = motivoDesligamento;
        VerbasRescisorias = verbasRescisorias;
        UpdatedAt = DateTime.UtcNow;
    }
}
