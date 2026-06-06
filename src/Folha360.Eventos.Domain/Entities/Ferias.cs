using Folha360.Domain;

namespace Folha360.Eventos.Domain.Entities;

/// <summary>
/// Entidade Ferias — representa a concessão de férias a um funcionário.
/// Schema: tenant.
/// </summary>
public class Ferias : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public int DiasGozo { get; private set; }
    public DateOnly PeriodoAquisitivoInicio { get; private set; }
    public DateOnly PeriodoAquisitivoFim { get; private set; }
    public TipoFerias TipoFerias { get; private set; }
    public string? XmlContent { get; set; }

    private Ferias()
    {
    }

    public Ferias(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataInicio,
        int diasGozo,
        DateOnly periodoAquisitivoInicio,
        DateOnly periodoAquisitivoFim,
        TipoFerias tipoFerias)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataInicio = dataInicio;
        DiasGozo = diasGozo;
        PeriodoAquisitivoInicio = periodoAquisitivoInicio;
        PeriodoAquisitivoFim = periodoAquisitivoFim;
        TipoFerias = tipoFerias;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly dataInicio,
        int diasGozo,
        DateOnly periodoAquisitivoInicio,
        DateOnly periodoAquisitivoFim,
        TipoFerias tipoFerias)
    {
        DataInicio = dataInicio;
        DiasGozo = diasGozo;
        PeriodoAquisitivoInicio = periodoAquisitivoInicio;
        PeriodoAquisitivoFim = periodoAquisitivoFim;
        TipoFerias = tipoFerias;
        UpdatedAt = DateTime.UtcNow;
    }
}
