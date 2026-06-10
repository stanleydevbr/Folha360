namespace Folha360.Relatorios.Domain.Entities;

public class RelatorioAgendamento
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; private set; }
    public TipoRelatorio TipoRelatorio { get; private set; }
    public FormatoExportacao Formato { get; private set; }
    public string Recorrencia { get; private set; } = string.Empty;
    public string Destinatarios { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private RelatorioAgendamento()
    {
    }

    public static RelatorioAgendamento Criar(
        Guid empresaId,
        TipoRelatorio tipoRelatorio,
        FormatoExportacao formato,
        string recorrencia,
        string destinatarios)
    {
        return new RelatorioAgendamento
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            TipoRelatorio = tipoRelatorio,
            Formato = formato,
            Recorrencia = recorrencia,
            Destinatarios = destinatarios,
            Ativo = true,
            CriadoEm = DateTime.UtcNow,
        };
    }

    public void Atualizar(string? recorrencia, FormatoExportacao? formato, string? destinatarios, bool? ativo)
    {
        if (recorrencia is not null)
        {
            Recorrencia = recorrencia;
        }

        if (formato.HasValue)
        {
            Formato = formato.Value;
        }

        if (destinatarios is not null)
        {
            Destinatarios = destinatarios;
        }

        if (ativo.HasValue)
        {
            Ativo = ativo.Value;
        }

        AtualizadoEm = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Ativo = false;
        DeletedAt = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }
}
