using System.Text.Json;
using Folha360.Relatorios.Application.Commands;
using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Folha360.Relatorios.Infrastructure.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IAgendamentoRepository _repository;
    private readonly ILogger<AgendamentoService> _logger;

    public AgendamentoService(
        ISchedulerFactory schedulerFactory,
        IAgendamentoRepository repository,
        ILogger<AgendamentoService> logger)
    {
        _schedulerFactory = schedulerFactory;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> CriarAsync(CriarAgendamentoCommand cmd, CancellationToken ct)
    {
        var agendamento = RelatorioAgendamento.Criar(
            cmd.EmpresaId,
            cmd.TipoRelatorio,
            cmd.Formato,
            cmd.Recorrencia,
            JsonSerializer.Serialize(cmd.Destinatarios));

        await _repository.AdicionarAsync(agendamento, ct);
        await AgendarJobAsync(agendamento, ct);

        _logger.LogInformation("Agendamento criado: {Id}", agendamento.Id);
        return agendamento.Id;
    }

    public async Task AtualizarAsync(AtualizarAgendamentoCommand cmd, CancellationToken ct)
    {
        var agendamento = await _repository.ObterPorIdAsync(cmd.AgendamentoId, ct)
            ?? throw new InvalidOperationException($"Agendamento {cmd.AgendamentoId} não encontrado.");

        agendamento.Atualizar(
            cmd.Recorrencia,
            cmd.Formato,
            cmd.Destinatarios is not null ? JsonSerializer.Serialize(cmd.Destinatarios) : null,
            cmd.Ativo);

        await _repository.AtualizarAsync(agendamento, ct);

        // Reschedule if cron changed
        if (cmd.Recorrencia is not null || cmd.Ativo.HasValue)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);
            var triggerKey = new TriggerKey($"agendamento_{agendamento.Id}");

            if (await scheduler.CheckExists(triggerKey, ct))
                await scheduler.UnscheduleJob(triggerKey, ct);

            if (agendamento.Ativo)
                await AgendarJobAsync(agendamento, ct);
        }

        _logger.LogInformation("Agendamento atualizado: {Id}", agendamento.Id);
    }

    public async Task CancelarAsync(Guid agendamentoId, CancellationToken ct)
    {
        var agendamento = await _repository.ObterPorIdAsync(agendamentoId, ct)
            ?? throw new InvalidOperationException($"Agendamento {agendamentoId} não encontrado.");

        agendamento.Cancelar();
        await _repository.AtualizarAsync(agendamento, ct);

        var scheduler = await _schedulerFactory.GetScheduler(ct);
        var triggerKey = new TriggerKey($"agendamento_{agendamentoId}");
        if (await scheduler.CheckExists(triggerKey, ct))
            await scheduler.UnscheduleJob(triggerKey, ct);

        _logger.LogInformation("Agendamento cancelado: {Id}", agendamentoId);
    }

    public async Task ExecutarAsync(Guid agendamentoId, CancellationToken ct)
    {
        var agendamento = await _repository.ObterPorIdAsync(agendamentoId, ct)
            ?? throw new InvalidOperationException($"Agendamento {agendamentoId} não encontrado.");

        var scheduler = await _schedulerFactory.GetScheduler(ct);
        var jobKey = new JobKey($"job_agendamento_{agendamentoId}");

        var dataMap = new JobDataMap
        {
            { "agendamento_id", agendamento.Id.ToString() },
            { "empresa_id", agendamento.EmpresaId.ToString() },
            { "tipo_relatorio", agendamento.TipoRelatorio.ToString() },
            { "formato", agendamento.Formato.ToString() },
            { "destinatarios", agendamento.Destinatarios },
        };

        var job = JobBuilder.Create<Jobs.GerarRelatorioJob>()
            .WithIdentity(jobKey)
            .UsingJobData(dataMap)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"trigger_manual_{agendamentoId}")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger, ct);
        _logger.LogInformation("Execução manual disparada para agendamento: {Id}", agendamentoId);
    }

    private async Task AgendarJobAsync(RelatorioAgendamento agendamento, CancellationToken ct)
    {
        if (!agendamento.Ativo)
        {
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(ct);
        var jobKey = new JobKey($"job_agendamento_{agendamento.Id}");

        var dataMap = new JobDataMap
        {
            { "agendamento_id", agendamento.Id.ToString() },
            { "empresa_id", agendamento.EmpresaId.ToString() },
            { "tipo_relatorio", agendamento.TipoRelatorio.ToString() },
            { "formato", agendamento.Formato.ToString() },
            { "destinatarios", agendamento.Destinatarios },
        };

        var job = JobBuilder.Create<Jobs.GerarRelatorioJob>()
            .WithIdentity(jobKey)
            .UsingJobData(dataMap)
            .StoreDurably()
            .Build();

        await scheduler.AddJob(job, true, ct);

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"agendamento_{agendamento.Id}")
            .WithCronSchedule(agendamento.Recorrencia)
            .ForJob(jobKey)
            .Build();

        if (await scheduler.CheckExists(trigger.Key, ct))
            await scheduler.RescheduleJob(trigger.Key, trigger, ct);
        else
            await scheduler.ScheduleJob(trigger, ct);

        _logger.LogInformation("Job agendado: {Id} com cron '{Cron}'", agendamento.Id, agendamento.Recorrencia);
    }
}
