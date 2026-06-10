using System.Diagnostics.Metrics;

namespace Folha360.Relatorios.Infrastructure.Metrics;

public class RelatoriosMetrics
{
    private readonly Counter<int> _relatoriosGerados;
    private readonly Histogram<double> _tempoGeracao;
    private readonly Histogram<double> _loteTempo;
    private readonly Counter<int> _emailsEnviados;
    private readonly Counter<int> _emailsFalhas;
    private readonly Histogram<double> _mviewRefreshTempo;
    private readonly Counter<int> _agendamentosExecutados;

    public RelatoriosMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Folha360.Relatorios");

        _relatoriosGerados = meter.CreateCounter<int>(
            "folha360_relatorios_gerados_total",
            description: "Total de relatórios gerados");

        _tempoGeracao = meter.CreateHistogram<double>(
            "folha360_relatorios_tempo_geracao_segundos",
            unit: "s",
            description: "Tempo de geração por tipo de relatório");

        _loteTempo = meter.CreateHistogram<double>(
            "folha360_relatorios_lote_tempo_segundos",
            unit: "s",
            description: "Tempo total de geração de lote de holerites");

        _emailsEnviados = meter.CreateCounter<int>(
            "folha360_relatorios_emails_enviados_total",
            description: "Total de emails enviados");

        _emailsFalhas = meter.CreateCounter<int>(
            "folha360_relatorios_emails_falhas_total",
            description: "Falhas de envio de email");

        _mviewRefreshTempo = meter.CreateHistogram<double>(
            "folha360_relatorios_mview_refresh_tempo_segundos",
            unit: "s",
            description: "Tempo de refresh de materialized views");

        _agendamentosExecutados = meter.CreateCounter<int>(
            "folha360_relatorios_agendamentos_executados_total",
            description: "Total de execuções de agendamento");
    }

    public void RegistrarRelatorioGerado(string tipo, string formato)
    {
        _relatoriosGerados.Add(1,
            new KeyValuePair<string, object?>("tipo", tipo),
            new KeyValuePair<string, object?>("formato", formato));
    }

    public void RegistrarTempoGeracao(string tipo, double segundos)
    {
        _tempoGeracao.Record(segundos,
            new KeyValuePair<string, object?>("tipo", tipo));
    }

    public void RegistrarTempoLote(double segundos)
    {
        _loteTempo.Record(segundos);
    }

    public void RegistrarEmailEnviado(string status)
    {
        _emailsEnviados.Add(1,
            new KeyValuePair<string, object?>("status", status));
    }

    public void RegistrarEmailFalha()
    {
        _emailsFalhas.Add(1);
    }

    public void RegistrarTempoRefreshView(double segundos)
    {
        _mviewRefreshTempo.Record(segundos);
    }

    public void RegistrarExecucaoAgendamento(string status)
    {
        _agendamentosExecutados.Add(1,
            new KeyValuePair<string, object?>("status", status));
    }
}
