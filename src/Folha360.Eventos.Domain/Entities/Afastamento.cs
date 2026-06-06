using Folha360.Domain.Attributes;
using Folha360.Domain;

namespace Folha360.Eventos.Domain.Entities;

/// <summary>
/// Entidade Afastamento — representa um afastamento temporário de um funcionário.
/// Schema: tenant.
/// </summary>
public class Afastamento : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly DataFimPrevista { get; private set; }
    public DateOnly? DataFimEfetiva { get; private set; }
    public TipoAfastamento TipoAfastamento { get; private set; }

    [SensitiveData]
    public string? Cid { get; private set; }

    public string? XmlContent { get; set; }

    private Afastamento()
    {
    }

    public Afastamento(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataInicio,
        DateOnly dataFimPrevista,
        TipoAfastamento tipoAfastamento,
        string? cid = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataInicio = dataInicio;
        DataFimPrevista = dataFimPrevista;
        TipoAfastamento = tipoAfastamento;
        Cid = cid;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly dataInicio,
        DateOnly dataFimPrevista,
        TipoAfastamento tipoAfastamento,
        DateOnly? dataFimEfetiva = null,
        string? cid = null)
    {
        DataInicio = dataInicio;
        DataFimPrevista = dataFimPrevista;
        TipoAfastamento = tipoAfastamento;
        DataFimEfetiva = dataFimEfetiva;
        Cid = cid;
        UpdatedAt = DateTime.UtcNow;
    }
}
