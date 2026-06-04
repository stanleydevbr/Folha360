using Folha360.Domain.Entities;

namespace Folha360.Domain.Abstractions;

public interface IAuditLogRepository
{
    Task LogAsync(AuditLogEntry entry, CancellationToken ct = default);
}
