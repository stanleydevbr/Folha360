using Folha360.Relatorios.Domain.Entities;

namespace Folha360.Relatorios.Domain.Abstractions;

public interface IAgendamentoRepository
{
    Task<RelatorioAgendamento?> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<RelatorioAgendamento>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct);
    Task<IReadOnlyList<RelatorioAgendamento>> ListarAtivosAsync(CancellationToken ct);
    Task AdicionarAsync(RelatorioAgendamento agendamento, CancellationToken ct);
    Task AtualizarAsync(RelatorioAgendamento agendamento, CancellationToken ct);
    Task AdicionarExecucaoAsync(RelatorioExecucao execucao, CancellationToken ct);
    Task<IReadOnlyList<RelatorioExecucao>> ListarExecucoesAsync(Guid agendamentoId, CancellationToken ct);
    Task<IReadOnlyList<RelatorioArquivo>> ListarArquivosAsync(Guid empresaId, string periodo, CancellationToken ct);
    Task AdicionarArquivoAsync(RelatorioArquivo arquivo, CancellationToken ct);
    Task InvalidarArquivosAsync(Guid empresaId, string periodo, CancellationToken ct);
}
