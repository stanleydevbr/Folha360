using Folha360.Domain;

namespace Folha360.Eventos.Domain.Entities;

/// <summary>
/// Entidade AlteracaoContratual — representa uma alteração contratual de um funcionário.
/// Schema: tenant.
/// </summary>
public class AlteracaoContratual : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public DateOnly DataAlteracao { get; private set; }
    public string? CamposAlterados { get; private set; }
    public string? ValorAnterior { get; private set; }
    public string? ValorNovo { get; private set; }
    public string? XmlContent { get; set; }

    private AlteracaoContratual()
    {
    }

    public AlteracaoContratual(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataAlteracao,
        string? camposAlterados = null,
        string? valorAnterior = null,
        string? valorNovo = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataAlteracao = dataAlteracao;
        CamposAlterados = camposAlterados;
        ValorAnterior = valorAnterior;
        ValorNovo = valorNovo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly dataAlteracao,
        string? camposAlterados = null,
        string? valorAnterior = null,
        string? valorNovo = null)
    {
        DataAlteracao = dataAlteracao;
        CamposAlterados = camposAlterados;
        ValorAnterior = valorAnterior;
        ValorNovo = valorNovo;
        UpdatedAt = DateTime.UtcNow;
    }
}
