using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Relatorios.Infrastructure.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly RelatoriosDbContext _context;

    public AgendamentoRepository(RelatoriosDbContext context)
    {
        _context = context;
    }

    public async Task<RelatorioAgendamento?> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.RelatorioAgendamentos
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<IReadOnlyList<RelatorioAgendamento>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct)
    {
        return await _context.RelatorioAgendamentos
            .Where(a => a.EmpresaId == empresaId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RelatorioAgendamento>> ListarAtivosAsync(CancellationToken ct)
    {
        return await _context.RelatorioAgendamentos
            .Where(a => a.Ativo)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(RelatorioAgendamento agendamento, CancellationToken ct)
    {
        await _context.RelatorioAgendamentos.AddAsync(agendamento, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(RelatorioAgendamento agendamento, CancellationToken ct)
    {
        _context.RelatorioAgendamentos.Update(agendamento);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AdicionarExecucaoAsync(RelatorioExecucao execucao, CancellationToken ct)
    {
        await _context.RelatorioExecucoes.AddAsync(execucao, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<RelatorioExecucao>> ListarExecucoesAsync(Guid agendamentoId, CancellationToken ct)
    {
        return await _context.RelatorioExecucoes
            .Where(e => e.AgendamentoId == agendamentoId)
            .OrderByDescending(e => e.IniciadoEm)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RelatorioArquivo>> ListarArquivosAsync(Guid empresaId, string periodo, CancellationToken ct)
    {
        return await _context.RelatorioArquivos
            .Where(a => a.EmpresaId == empresaId && a.Periodo == periodo)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync(ct);
    }

    public async Task AdicionarArquivoAsync(RelatorioArquivo arquivo, CancellationToken ct)
    {
        await _context.RelatorioArquivos.AddAsync(arquivo, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task InvalidarArquivosAsync(Guid empresaId, string periodo, CancellationToken ct)
    {
        var arquivos = await _context.RelatorioArquivos
            .IgnoreQueryFilters()
            .Where(a => a.EmpresaId == empresaId && a.Periodo == periodo && a.DeletedAt == null)
            .ToListAsync(ct);

        foreach (var arquivo in arquivos)
        {
            arquivo.Invalidar();
        }

        await _context.SaveChangesAsync(ct);
    }
}
