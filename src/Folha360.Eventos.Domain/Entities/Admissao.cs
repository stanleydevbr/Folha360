using Folha360.Domain;

namespace Folha360.Eventos.Domain.Entities;

/// <summary>
/// Entidade Admissao — representa o evento de admissão de um funcionário.
/// Schema: tenant.
/// </summary>
public class Admissao : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public DateOnly DataAdmissao { get; private set; }
    public Guid CargoId { get; private set; }
    public decimal SalarioInicial { get; private set; }
    public TipoContrato TipoContrato { get; private set; }
    public int? PeriodoExperienciaMeses { get; private set; }
    public string? XmlContent { get; set; }

    private Admissao()
    {
    }

    public Admissao(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataAdmissao,
        Guid cargoId,
        decimal salarioInicial,
        TipoContrato tipoContrato,
        int? periodoExperienciaMeses = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataAdmissao = dataAdmissao;
        CargoId = cargoId;
        SalarioInicial = salarioInicial;
        TipoContrato = tipoContrato;
        PeriodoExperienciaMeses = periodoExperienciaMeses;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        DateOnly dataAdmissao,
        Guid cargoId,
        decimal salarioInicial,
        TipoContrato tipoContrato,
        int? periodoExperienciaMeses = null)
    {
        DataAdmissao = dataAdmissao;
        CargoId = cargoId;
        SalarioInicial = salarioInicial;
        TipoContrato = tipoContrato;
        PeriodoExperienciaMeses = periodoExperienciaMeses;
        UpdatedAt = DateTime.UtcNow;
    }
}
