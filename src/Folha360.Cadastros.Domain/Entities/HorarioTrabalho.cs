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

    private HorarioTrabalho()
    {
    }

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

        Validate();
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

        Validate();
    }

    public override void Validate()
    {
        if (CargaHorariaDiaria > 600)
        {
            Notification.AddError("CARGA_HORARIA_EXCEDIDA",
                "Carga horária diária não pode exceder 10 horas (600 minutos).",
                nameof(CargaHorariaDiaria));
        }

        if (CargaHorariaDiaria > 360)
        {
            if (!InicioIntervalo.HasValue || !FimIntervalo.HasValue)
            {
                Notification.AddError("INTERVALO_OBRIGATORIO",
                    "Jornadas acima de 6 horas exigem intervalo definido.");
            }
            else
            {
                var duracaoIntervalo = (int)(FimIntervalo.Value - InicioIntervalo.Value).TotalMinutes;
                if (duracaoIntervalo < 60)
                {
                    Notification.AddError("INTERVALO_MINIMO",
                        "Intervalo deve ter no mínimo 60 minutos para jornadas acima de 6 horas.");
                }
            }
        }
    }
}
