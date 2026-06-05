using Folha360.Cadastros.Domain.Attributes;
using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Dependente — dependentes do funcionário para IRRF, salário-família e pensão.
/// Schema: tenant.
/// </summary>
public class Dependente : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public string Nome { get; private set; } = null!;

    [SensitiveData]
    public string Cpf { get; private set; } = null!;

    public DateOnly DataNascimento { get; private set; }
    public string Tipo { get; private set; } = null!;
    public string? GrauParentesco { get; private set; }
    public bool DependenteIrrf { get; private set; }
    public bool DependenteSalarioFamilia { get; private set; }
    public decimal? PensaoAlimenticiaValor { get; private set; }
    public decimal? PensaoAlimenticiaPercentual { get; private set; }

#pragma warning disable CS8618
    private Dependente()
    {
    }
#pragma warning restore CS8618

    public Dependente(
        Guid funcionarioId,
        string nome,
        string cpf,
        DateOnly dataNascimento,
        string tipo,
        string? grauParentesco = null,
        bool dependenteIrrf = false,
        bool dependenteSalarioFamilia = false,
        decimal? pensaoAlimenticiaValor = null,
        decimal? pensaoAlimenticiaPercentual = null)
    {
        if (dependenteSalarioFamilia)
        {
            var idade = DateTime.Today.Year - dataNascimento.Year;
            if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-idade)))
                idade--;

            if (idade > 14)
                throw new ArgumentException("Dependentes para salário-família devem ter idade ≤ 14 anos.");
        }

        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        Nome = nome;
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Tipo = tipo;
        GrauParentesco = grauParentesco;
        DependenteIrrf = dependenteIrrf;
        DependenteSalarioFamilia = dependenteSalarioFamilia;
        PensaoAlimenticiaValor = pensaoAlimenticiaValor;
        PensaoAlimenticiaPercentual = pensaoAlimenticiaPercentual;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        DateOnly dataNascimento,
        string tipo,
        string? grauParentesco = null,
        bool? dependenteIrrf = null,
        bool? dependenteSalarioFamilia = null,
        decimal? pensaoAlimenticiaValor = null,
        decimal? pensaoAlimenticiaPercentual = null)
    {
        if (dependenteSalarioFamilia ?? DependenteSalarioFamilia)
        {
            var idade = DateTime.Today.Year - dataNascimento.Year;
            if (dataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-idade)))
                idade--;

            if (idade > 14)
                throw new ArgumentException("Dependentes para salário-família devem ter idade ≤ 14 anos.");
        }

        Nome = nome;
        DataNascimento = dataNascimento;
        Tipo = tipo;
        GrauParentesco = grauParentesco;
        DependenteIrrf = dependenteIrrf ?? DependenteIrrf;
        DependenteSalarioFamilia = dependenteSalarioFamilia ?? DependenteSalarioFamilia;
        PensaoAlimenticiaValor = pensaoAlimenticiaValor;
        PensaoAlimenticiaPercentual = pensaoAlimenticiaPercentual;
        UpdatedAt = DateTime.UtcNow;
    }
}
