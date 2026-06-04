using Folha360.Application.Commands;
using FluentValidation.TestHelper;

namespace Folha360.Tests.Application;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_WhenEmailIsEmpty()
    {
        var command = new LoginCommand(string.Empty, "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_WhenEmailIsInvalid()
    {
        var command = new LoginCommand("invalid-email", "password");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_WhenPasswordIsEmpty()
    {
        var command = new LoginCommand("admin@test.com", string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_NotHaveError_WhenCommandIsValid()
    {
        var command = new LoginCommand("admin@folha360.com.br", "Admin@123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
