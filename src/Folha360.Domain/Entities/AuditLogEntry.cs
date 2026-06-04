namespace Folha360.Domain.Entities;

public sealed class AuditLogEntry
{
    public long Id { get; private set; }
    public string SchemaName { get; private set; } = string.Empty;
    public string TableName { get; private set; } = string.Empty;
    public Guid RecordId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldData { get; private set; }
    public string? NewData { get; private set; }
    public Guid ChangedBy { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private AuditLogEntry()
    {
    }

    public AuditLogEntry(
        string schemaName,
        string tableName,
        Guid recordId,
        string action,
        string? oldData,
        string? newData,
        Guid changedBy)
    {
        SchemaName = schemaName;
        TableName = tableName;
        RecordId = recordId;
        Action = action;
        OldData = oldData;
        NewData = newData;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }
}
