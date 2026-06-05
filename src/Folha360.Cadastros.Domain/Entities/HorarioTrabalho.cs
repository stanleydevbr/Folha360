using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade HorarioTrabalho — horários e turnos de trabalho, compatível com S-1050 do e-Social.
/// Schema: tenant. Carga diária ≤ 600min; intervalo ≥ 60min se jornada > 6h.
/// </summary>
public class HorarioTrabalho : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Codigo { get; private set; } = null!;
    public string Descricao { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;
    public int CargaHorariaDiaria { get; private set; }
    public int CargaHorariaSemanal { get; private set; }
    public TimeOnly InicioJornada { get; private set; }
    public TimeOnly FimJornada { get; private set; }
    public TimeOnly? InicioIntervalo { get; private set; }
    public TimeOnly? FimIntervalo { get; private set; }
    public int ToleranciaAtrasoMinutos { get; private set; }

#pragma warning disable CS8618
    private HorarioTrabalho()
    {
    }
#pragma warning restore CS8618

    public HorarioTrabalho(
        Guid empresaId,
        string codigo,
        string descricao,
        string tipo,
        int cargaHorariaDiaria,
        int cargaHorariaSemanal,
        TimeOnly inicioJornada,
        TimeOnly fimJornada,
        TimeOnly? inicioIntervalo = null,
        TimeOnly? fimIntervalo = null,
        int toleranciaAtrasoMinutos = 0)
    {
        if (cargaHorariaDiaria > 600)
            throw new ArgumentException("Carga horária diária não pode exceder 10 horas (600 minutos).");

        if (cargaHorariaDiaria > 360)
        {
            if (!inicioIntervalo.HasValue || !fimIntervalo.HasValue)
                throw new ArgumentException("Jornadas acima de 6 horas exigem intervalo definido.");

            var duracaoIntervalo = (int)(fimIntervalo.Value - inicioIntervalo.Value).TotalMinutes;
            if (duracaoIntervalo < 60)
                throw new ArgumentException("Intervalo deve ter no mínimo 60 minutos para jornadas acima de 6 horas.");
        }

        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Codigo = codigo;
        Descricao = descricao;
        Tipo = tipo;
        CargaHorariaDiaria = cargaHorariaDiaria;
        CargaHorariaSemanal = cargaHorariaSemanal;
        InicioJornada = inicioJornada;
        FimJornada = fimJornada;
        InicioIntervalo = inicioIntervalo;
        FimIntervalo = fimIntervalo;
        ToleranciaAtrasoMinutos = toleranciaAtrasoMinutos;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        string tipo,
        int cargaHorariaDiaria,
        int cargaHorariaSemanal,
        TimeOnly inicioJornada,
        TimeOnly fimJornada,
        TimeOnly? inicioIntervalo = null,
        TimeOnly? fimIntervalo = null,
        int? toleranciaAtrasoMinutos = null)
    {
        if (cargaHorariaDiaria > 600)
            throw new ArgumentException("Carga horária diária não pode exceder 10 horas (600 minutos).");

        if (cargaHorariaDiaria > 360)
        {
            if (!inicioIntervalo.HasValue || !fimIntervalo.HasValue)
                throw new ArgumentException("Jornadas acima de 6 horas exigem intervalo definido.");

            var duracaoIntervalo = (int)(fimIntervalo.Value - inicioIntervalo.Value).TotalMinutes;
            if (duracaoIntervalo < 60)
                throw new ArgumentException("Intervalo deve ter no mínimo 60 minutos para jornadas acima de 6 horas.");
        }

        Descricao = descricao;
        Tipo = tipo;
        CargaHorariaDiaria = cargaHorariaDiaria;
        CargaHorariaSemanal = cargaHorariaSemanal;
        InicioJornada = inicioJornada;
        FimJornada = fimJornada;
        InicioIntervalo = inicioIntervalo;
        FimIntervalo = fimIntervalo;
        ToleranciaAtrasoMinutos = toleranciaAtrasoMinutos ?? ToleranciaAtrasoMinutos;
        UpdatedAt = DateTime.UtcNow;
    }
}
