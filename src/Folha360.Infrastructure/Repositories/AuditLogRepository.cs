using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Folha360.Infrastructure.Data;

namespace Folha360.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly Folha360DbContext _context;

    public AuditLogRepository(Folha360DbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(AuditLogEntry entry, CancellationToken ct = default)
    {
        _context.AuditLogs.Add(entry);
        await _context.SaveChangesAsync(ct);
    }
}
