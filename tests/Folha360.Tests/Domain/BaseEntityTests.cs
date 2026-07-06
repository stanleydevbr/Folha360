using Folha360.Domain;
using Folha360.Domain.Validation;

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

    [Fact]
    public void BaseEntity_NewInstance_ShouldHaveEmptyNotification()
    {
        var entity = new TestEntity();

        Assert.NotNull(entity.Notification);
        Assert.Empty(entity.Notification.Errors);
        Assert.True(entity.IsValid);
    }

    [Fact]
    public void BaseEntity_Validate_ShouldNotThrow()
    {
        var entity = new TestEntity();

        var exception = Record.Exception(() => entity.Validate());

        Assert.Null(exception);
    }

    [Fact]
    public void BaseEntity_Validate_ShouldRemainValidWhenNoOverrides()
    {
        var entity = new TestEntity();

        entity.Validate();

        Assert.True(entity.IsValid);
        Assert.Empty(entity.Notification.Errors);
    }

    [Fact]
    public void BaseEntity_ConcreteEntity_CanAddNotificationsViaValidate()
    {
        var entity = new ValidatableTestEntity("");

        entity.Validate();

        Assert.False(entity.IsValid);
        Assert.Single(entity.Notification.Errors);
        Assert.Equal("NOME_OBRIGATORIO", entity.Notification.Errors[0].Code);
    }

    [Fact]
    public void BaseEntity_ConcreteEntity_ValidState_NoNotifications()
    {
        var entity = new ValidatableTestEntity("João");

        entity.Validate();

        Assert.True(entity.IsValid);
        Assert.Empty(entity.Notification.Errors);
    }

    private class TestEntity : BaseEntity
    {
    }

    private class ValidatableTestEntity : BaseEntity
    {
        private readonly string _nome;

        public ValidatableTestEntity(string nome)
        {
            _nome = nome;
        }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(_nome))
                Notification.AddError("NOME_OBRIGATORIO", "Nome é obrigatório.", nameof(_nome));
        }
    }
}
