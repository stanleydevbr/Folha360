using Folha360.Domain;

namespace Folha360.Tests.Infrastructure;

[Trait("Category", "Unit")]
public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_Should_ReturnConsistentHash()
    {
        var hash1 = PasswordHelper.HashPassword("Admin@123");
        var hash2 = PasswordHelper.HashPassword("Admin@123");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Should_ReturnDifferentHashForDifferentPasswords()
    {
        var hash1 = PasswordHelper.HashPassword("Admin@123");
        var hash2 = PasswordHelper.HashPassword("Oper@123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Should_NotReturnEmpty()
    {
        var hash = PasswordHelper.HashPassword("Admin@123");
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void VerifyPassword_Should_ReturnTrue_ForCorrectPassword()
    {
        var hash = PasswordHelper.HashPassword("Admin@123");
        Assert.True(PasswordHelper.VerifyPassword("Admin@123", hash));
    }

    [Fact]
    public void VerifyPassword_Should_ReturnFalse_ForWrongPassword()
    {
        var hash = PasswordHelper.HashPassword("Admin@123");
        Assert.False(PasswordHelper.VerifyPassword("WrongPassword", hash));
    }
}
