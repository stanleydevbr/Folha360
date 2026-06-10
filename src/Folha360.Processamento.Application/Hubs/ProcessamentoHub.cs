using Microsoft.AspNetCore.SignalR;

namespace Folha360.Processamento.Application.Hubs;

public class ProcessamentoHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var empresaId = Context.GetHttpContext()?.Request.Query["empresaId"].FirstOrDefault();
        if (!string.IsNullOrEmpty(empresaId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"empresa_{empresaId}");
        }

        await base.OnConnectedAsync();
    }

    public async Task ProcessingStarted(Guid processamentoId)
    {
        await Clients.Caller.SendAsync("ProcessingStarted", processamentoId);
    }

    public async Task ProgressUpdated(Guid processamentoId, int percentual, int processados, int erros)
    {
        await Clients.Caller.SendAsync("ProgressUpdated", processamentoId, percentual, processados, erros);
    }

    public async Task ProcessingCompleted(Guid processamentoId, decimal totalLiquido)
    {
        await Clients.Caller.SendAsync("ProcessingCompleted", processamentoId, totalLiquido);
    }

    public async Task ProcessingFailed(Guid processamentoId, string erro)
    {
        await Clients.Caller.SendAsync("ProcessingFailed", processamentoId, erro);
    }

    public async Task ProcessingReopened(Guid processamentoId, int versao)
    {
        await Clients.Caller.SendAsync("ProcessingReopened", processamentoId, versao);
    }
}
