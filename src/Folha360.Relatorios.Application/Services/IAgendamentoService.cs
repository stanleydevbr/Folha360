using Folha360.Relatorios.Application.Commands;

namespace Folha360.Relatorios.Application.Services;

public interface IAgendamentoService
{
    Task<Guid> CriarAsync(CriarAgendamentoCommand cmd, CancellationToken ct);
    Task AtualizarAsync(AtualizarAgendamentoCommand cmd, CancellationToken ct);
    Task CancelarAsync(Guid agendamentoId, CancellationToken ct);
    Task ExecutarAsync(Guid agendamentoId, CancellationToken ct);
}
