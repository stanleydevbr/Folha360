using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Fiscais.Infrastructure.Repositories;

public class GuiaRecolhimentoRepository : IGuiaRecolhimentoRepository
{
    private readonly FiscaisDbContext _context;

    public GuiaRecolhimentoRepository(FiscaisDbContext context)
    {
        _context = context;
    }

    public async Task<GuiaRecolhimento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.GuiasRecolhimento.FindAsync([id], ct);

    public async Task<IReadOnlyList<GuiaRecolhimento>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default)
        => await _context.GuiasRecolhimento
            .Where(g => g.EmpresaId == empresaId && g.Periodo == periodo)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<GuiaRecolhimento>> GetPendentesAsync(Guid empresaId, CancellationToken ct = default)
        => await _context.GuiasRecolhimento
            .Where(g => g.EmpresaId == empresaId && g.Status == StatusGuia.Pendente)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<GuiaRecolhimento>> GetVencidasAsync(Guid empresaId, CancellationToken ct = default)
        => await _context.GuiasRecolhimento
            .Where(g => g.EmpresaId == empresaId && g.Status == StatusGuia.Vencida)
            .ToListAsync(ct);

    public async Task AddAsync(GuiaRecolhimento guia, CancellationToken ct = default)
    {
        await _context.GuiasRecolhimento.AddAsync(guia, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(GuiaRecolhimento guia, CancellationToken ct = default)
    {
        _context.GuiasRecolhimento.Update(guia);
        await _context.SaveChangesAsync(ct);
    }
}
