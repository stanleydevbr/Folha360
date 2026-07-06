using Folha360.Domain.Validation;

namespace Folha360.Tests.Domain.Validation;

[Trait("Category", "Unit")]
public class NotificationTests
{
    [Fact]
    public void Notification_NewInstance_ShouldBeEmpty()
    {
        var notification = new Notification();

        Assert.Empty(notification.Errors);
        Assert.False(notification.HasErrors);
        Assert.True(notification.IsValid);
    }

    [Fact]
    public void AddError_SingleError_ShouldSetHasErrors()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Erro de teste.");

        Assert.Single(notification.Errors);
        Assert.True(notification.HasErrors);
        Assert.False(notification.IsValid);
    }

    [Fact]
    public void AddError_MultipleErrors_ShouldAccumulateAll()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Primeiro erro.");
        notification.AddError("ERR02", "Segundo erro.");
        notification.AddError("ERR03", "Terceiro erro.");

        Assert.Equal(3, notification.Errors.Count);
        Assert.True(notification.HasErrors);
    }

    [Fact]
    public void AddError_WithDefaultCode_ShouldUseValidation()
    {
        var notification = new Notification();
        notification.AddError("Apenas uma mensagem.");

        var error = Assert.Single(notification.Errors);
        Assert.Equal("VALIDATION", error.Code);
        Assert.Equal("Apenas uma mensagem.", error.Message);
        Assert.Null(error.MemberName);
    }

    [Fact]
    public void AddError_WithMemberName_ShouldStoreIt()
    {
        var notification = new Notification();
        notification.AddError("CPF_INVALIDO", "CPF inválido.", "Cpf");

        var error = Assert.Single(notification.Errors);
        Assert.Equal("CPF_INVALIDO", error.Code);
        Assert.Equal("CPF inválido.", error.Message);
        Assert.Equal("Cpf", error.MemberName);
    }

    [Fact]
    public void AddErrors_MultipleErrors_ShouldAddAll()
    {
        var notification = new Notification();
        var errors = new[]
        {
            new NotificationError("ERR01", "Erro 1"),
            new NotificationError("ERR02", "Erro 2"),
        };

        notification.AddErrors(errors);

        Assert.Equal(2, notification.Errors.Count);
    }

    [Fact]
    public void AddNotification_Merge_ShouldCombineErrors()
    {
        var notification1 = new Notification();
        notification1.AddError("ERR01", "Erro da primeira.");

        var notification2 = new Notification();
        notification2.AddError("ERR02", "Erro da segunda.");

        notification1.AddNotification(notification2);

        Assert.Equal(2, notification1.Errors.Count);
    }

    [Fact]
    public void ToErrorMessage_MultipleErrors_ShouldConcatenate()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Primeiro erro.");
        notification.AddError("ERR02", "Segundo erro.");

        var message = notification.ToErrorMessage();

        Assert.Equal("Primeiro erro.; Segundo erro.", message);
    }

    [Fact]
    public void ToErrorMessage_NoErrors_ShouldBeEmpty()
    {
        var notification = new Notification();

        var message = notification.ToErrorMessage();

        Assert.Equal(string.Empty, message);
    }
}

[Trait("Category", "Unit")]
public class NotifiableValueObjectTests
{
    [Fact]
    public void Build_ValidValue_ShouldReturnInstance()
    {
        var builder = new TestValueObjectBuilder("valid");

        var result = builder.Build();

        Assert.NotNull(result);
        Assert.Equal("VALID", result);
        Assert.True(builder.IsValid);
    }

    [Fact]
    public void Build_InvalidValue_ShouldReturnNull()
    {
        var builder = new TestValueObjectBuilder(string.Empty);

        var result = builder.Build();

        Assert.Null(result);
        Assert.False(builder.IsValid);
        Assert.True(builder.Notification.HasErrors);
    }

    [Fact]
    public void Build_InvalidValue_ShouldHaveErrorNotification()
    {
        var builder = new TestValueObjectBuilder(string.Empty);

        builder.Build();

        var error = Assert.Single(builder.Notification.Errors);
        Assert.Equal("VALOR_INVALIDO", error.Code);
        Assert.Equal("Valor não pode ser vazio.", error.Message);
        Assert.Equal("Value", error.MemberName);
    }

    [Fact]
    public void Build_MultipleInvalidValues_ShouldAccumulateAllErrors()
    {
        var builder = new MultiErrorTestBuilder();

        var result = builder.Build();

        Assert.Null(result);
        Assert.Equal(2, builder.Notification.Errors.Count);
        Assert.False(builder.IsValid);
    }

    // Usamos string como tipo de retorno para os builders de teste,
    // evitando problemas de acessibilidade com tipos aninhados.
    private class TestValueObjectBuilder : NotifiableValueObject<string>
    {
        private readonly string _input;

        public TestValueObjectBuilder(string input)
        {
            _input = input;
        }

        public override string? Build()
        {
            if (string.IsNullOrWhiteSpace(_input))
            {
                Notification.AddError("VALOR_INVALIDO", "Valor não pode ser vazio.", "Value");
                return null;
            }

            return _input.ToUpperInvariant();
        }
    }

    private class MultiErrorTestBuilder : NotifiableValueObject<string>
    {
        public override string? Build()
        {
            Notification.AddError("ERR01", "Primeiro erro.", "Campo1");
            Notification.AddError("ERR02", "Segundo erro.", "Campo2");
            return null;
        }
    }
}
