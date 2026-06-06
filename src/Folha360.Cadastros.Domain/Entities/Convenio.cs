using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade Convenio — convênios de benefícios (plano de saúde, odontológico, VR, VT, etc.).
/// Schema: tenant. Soma percentuais empresa + funcionário = 100%.
/// </summary>
public class Convenio : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;
    public string? Operadora { get; private set; }
    public decimal ValorMensal { get; private set; }
    public decimal PercentualEmpresa { get; private set; }
    public decimal PercentualFuncionario { get; private set; }

    private Convenio()
    {
    }

    public Convenio(
        Guid empresaId,
        string nome,
        string tipo,
        decimal valorMensal,
        decimal percentualEmpresa,
        decimal percentualFuncionario,
        string? operadora = null)
    {
        if (percentualEmpresa + percentualFuncionario != 100)
            throw new ArgumentException("A soma dos percentuais empresa + funcionário deve ser igual a 100%.");

        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Nome = nome;
        Tipo = tipo;
        ValorMensal = valorMensal;
        PercentualEmpresa = percentualEmpresa;
        PercentualFuncionario = percentualFuncionario;
        Operadora = operadora;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string nome,
        string tipo,
        decimal valorMensal,
        decimal percentualEmpresa,
        decimal percentualFuncionario,
        string? operadora = null)
    {
        if (percentualEmpresa + percentualFuncionario != 100)
            throw new ArgumentException("A soma dos percentuais empresa + funcionário deve ser igual a 100%.");

        Nome = nome;
        Tipo = tipo;
        ValorMensal = valorMensal;
        PercentualEmpresa = percentualEmpresa;
        PercentualFuncionario = percentualFuncionario;
        Operadora = operadora;
        UpdatedAt = DateTime.UtcNow;
    }
}
