using Folha360.Domain;
using Folha360.Domain.Entities;

namespace Folha360.Tests.Domain;

[Trait("Category", "Unit")]
public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_ShouldGenerateId()
    {
        var entity = new TestEntity();
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void BaseEntity_ShouldSetCreatedAt()
    {
        var entity = new TestEntity();
        Assert.NotEqual(default, entity.CreatedAt);
        var diff = Math.Abs((entity.CreatedAt - entity.UpdatedAt).TotalMilliseconds);
        Assert.True(diff < 10, "CreatedAt and UpdatedAt should be very close");
    }

    private class TestEntity : BaseEntity
    {
    }
}
