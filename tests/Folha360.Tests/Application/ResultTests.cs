using Folha360.Application;
using Folha360.Domain.Validation;

namespace Folha360.Tests.Application;

[Trait("Category", "Unit")]
public class ResultTests
{
    [Fact]
    public void FromNotification_WithErrors_ShouldReturnFailure()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Erro de teste.");

        var result = Result<string>.FromNotification(notification);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("ERR01", result.Errors[0].Code);
        Assert.Equal("Erro de teste.", result.Errors[0].Message);
    }

    [Fact]
    public void FromNotification_WithMultipleErrors_ShouldReturnAll()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Primeiro erro.");
        notification.AddError("ERR02", "Segundo erro.");

        var result = Result<string>.FromNotification(notification);

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void FromNotification_WithNoErrors_ShouldReturnSuccess()
    {
        var notification = new Notification();

        var result = Result<string>.FromNotification("valor", notification);

        Assert.True(result.IsSuccess);
        Assert.Equal("valor", result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void FromNotification_WithValueAndValidNotification_ShouldReturnSuccess()
    {
        var notification = new Notification();

        var result = Result<int>.FromNotification(42, notification);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void FromNotification_WithValueAndInvalidNotification_ShouldReturnFailure()
    {
        var notification = new Notification();
        notification.AddError("ERR01", "Erro de teste.");

        var result = Result<int>.FromNotification(42, notification);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void FromNotification_EmptyNotification_ShouldHaveNoMappedErrors()
    {
        var notification = new Notification();

        var result = Result<string>.FromNotification(notification);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void FromNotification_ShouldMapNotificationCodeAndMessage()
    {
        var notification = new Notification();
        notification.AddError("CPF_INVALIDO", "CPF inválido.", "Cpf");
        notification.AddError("NOME_OBRIGATORIO", "Nome é obrigatório.", "Nome");

        var result = Result<string>.FromNotification(notification);

        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("CPF_INVALIDO", result.Errors[0].Code);
        Assert.Equal("NOME_OBRIGATORIO", result.Errors[1].Code);

        // MemberName não é mapeado para Error (intencional — Error só tem Code/Message)
    }
}
