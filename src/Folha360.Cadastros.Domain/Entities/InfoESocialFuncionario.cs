using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade InfoESocialFuncionario — informações complementares para e-Social (1:1).
/// Schema: tenant. Não implementa ISoftDeletable.
/// </summary>
public class InfoESocialFuncionario : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public bool IndicadorDeficiencia { get; private set; }
    public string? TipoDeficiencia { get; private set; }
    public DateOnly? DataEmissaoLaudoDeficiencia { get; private set; }
    public bool Reservista { get; private set; }
    public bool PrimeiroEmprego { get; private set; }
    public bool TrabalhadorAposentado { get; private set; }
    public string? RegistroProfissional { get; private set; }

    private InfoESocialFuncionario()
        : base()
    {
    }

    public InfoESocialFuncionario(
        Guid funcionarioId,
        bool indicadorDeficiencia = false,
        string? tipoDeficiencia = null,
        DateOnly? dataEmissaoLaudoDeficiencia = null,
        bool reservista = false,
        bool primeiroEmprego = false,
        bool trabalhadorAposentado = false,
        string? registroProfissional = null)
    {
        if (indicadorDeficiencia && string.IsNullOrWhiteSpace(tipoDeficiencia))
        {
            throw new ArgumentException("Tipo de deficiência é obrigatório quando indicador é true.");
        }

        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        IndicadorDeficiencia = indicadorDeficiencia;
        TipoDeficiencia = tipoDeficiencia;
        DataEmissaoLaudoDeficiencia = dataEmissaoLaudoDeficiencia;
        Reservista = reservista;
        PrimeiroEmprego = primeiroEmprego;
        TrabalhadorAposentado = trabalhadorAposentado;
        RegistroProfissional = registroProfissional;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        bool? indicadorDeficiencia = null,
        string? tipoDeficiencia = null,
        DateOnly? dataEmissaoLaudoDeficiencia = null,
        bool? reservista = null,
        bool? primeiroEmprego = null,
        bool? trabalhadorAposentado = null,
        string? registroProfissional = null)
    {
        if (indicadorDeficiencia.HasValue)
        {
            IndicadorDeficiencia = indicadorDeficiencia.Value;
        }

        if (IndicadorDeficiencia && string.IsNullOrWhiteSpace(tipoDeficiencia))
        {
            throw new ArgumentException("Tipo de deficiência é obrigatório quando indicador é true.");
        }

        TipoDeficiencia = tipoDeficiencia;
        DataEmissaoLaudoDeficiencia = dataEmissaoLaudoDeficiencia;

        if (reservista.HasValue)
        {
            Reservista = reservista.Value;
        }

        if (primeiroEmprego.HasValue)
        {
            PrimeiroEmprego = primeiroEmprego.Value;
        }

        if (trabalhadorAposentado.HasValue)
        {
            TrabalhadorAposentado = trabalhadorAposentado.Value;
        }

        RegistroProfissional = registroProfissional;
        UpdatedAt = DateTime.UtcNow;
    }
}
