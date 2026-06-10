namespace Folha360.Relatorios.Domain.Entities;

public class RelatorioExecucao
{
    public Guid Id { get; private set; }
    public Guid AgendamentoId { get; private set; }
    public StatusExecucao Status { get; private set; }
    public DateTime IniciadoEm { get; private set; }
    public DateTime? ConcluidoEm { get; private set; }
    public string? LinkArquivo { get; private set; }
    public string? LogErros { get; private set; }

    private RelatorioExecucao()
    {
    }

    public static RelatorioExecucao Iniciar(Guid agendamentoId)
    {
        return new RelatorioExecucao
        {
            Id = Guid.NewGuid(),
            AgendamentoId = agendamentoId,
            Status = StatusExecucao.EmAndamento,
            IniciadoEm = DateTime.UtcNow,
        };
    }

    public void Concluir(string? linkArquivo = null)
    {
        Status = StatusExecucao.Concluido;
        ConcluidoEm = DateTime.UtcNow;
        LinkArquivo = linkArquivo;
    }

    public void Falhar(string logErros)
    {
        Status = StatusExecucao.Falha;
        ConcluidoEm = DateTime.UtcNow;
        LogErros = logErros;
    }
}
