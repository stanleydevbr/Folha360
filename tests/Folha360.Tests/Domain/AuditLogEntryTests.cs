using Folha360.Domain.Entities;

namespace Folha360.Tests.Domain;

public class AuditLogEntryTests
{
    [Fact]
    public void AuditLogEntry_ShouldCreateWithValidProperties()
    {
        var recordId = Guid.NewGuid();
        var changedBy = Guid.NewGuid();
        var entry = new AuditLogEntry(
            "public",
            "usuario",
            recordId,
            "INSERT",
            null,
            "{\"name\":\"test\"}",
            changedBy);

        Assert.Equal("public", entry.SchemaName);
        Assert.Equal("usuario", entry.TableName);
        Assert.Equal(recordId, entry.RecordId);
        Assert.Equal("INSERT", entry.Action);
        Assert.Null(entry.OldData);
        Assert.Equal("{\"name\":\"test\"}", entry.NewData);
        Assert.Equal(changedBy, entry.ChangedBy);
        Assert.NotEqual(default, entry.ChangedAt);
    }
}
