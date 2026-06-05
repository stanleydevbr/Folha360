using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Cargo — cargo/função com CBO compatível com e-Social.
/// Schema: tenant.
/// </summary>
public class Cargo : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Cbo { get; private set; } = null!;
    public string? Descricao { get; private set; }
    public decimal? SalarioBaseMinimo { get; private set; }
    public decimal? SalarioBaseMaximo { get; private set; }

    private Cargo()
    {
    }

    public Cargo(
        Guid empresaId,
        string nome,
        string cbo,
        string? descricao = null,
        decimal? salarioBaseMinimo = null,
        decimal? salarioBaseMaximo = null)
    {
        if (salarioBaseMinimo.HasValue && salarioBaseMaximo.HasValue && salarioBaseMinimo > salarioBaseMaximo)
            throw new ArgumentException("Salário base mínimo não pode ser superior ao salário base máximo.");

        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Nome = nome;
        Cbo = cbo;
        Descricao = descricao;
        SalarioBaseMinimo = salarioBaseMinimo;
        SalarioBaseMaximo = salarioBaseMaximo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        string cbo,
        string? descricao = null,
        decimal? salarioBaseMinimo = null,
        decimal? salarioBaseMaximo = null)
    {
        if (salarioBaseMinimo.HasValue && salarioBaseMaximo.HasValue && salarioBaseMinimo > salarioBaseMaximo)
            throw new ArgumentException("Salário base mínimo não pode ser superior ao salário base máximo.");

        Nome = nome;
        Cbo = cbo;
        Descricao = descricao;
        SalarioBaseMinimo = salarioBaseMinimo;
        SalarioBaseMaximo = salarioBaseMaximo;
        UpdatedAt = DateTime.UtcNow;
    }
}
