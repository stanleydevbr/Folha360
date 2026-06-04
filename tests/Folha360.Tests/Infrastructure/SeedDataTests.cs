using Folha360.Infrastructure.Data;

namespace Folha360.Tests.Infrastructure;

public class SeedDataTests
{
    [Fact]
    public void HashPassword_Should_ReturnConsistentHash()
    {
        var hash1 = SeedData.HashPassword("Admin@123");
        var hash2 = SeedData.HashPassword("Admin@123");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Should_ReturnDifferentHashForDifferentPasswords()
    {
        var hash1 = SeedData.HashPassword("Admin@123");
        var hash2 = SeedData.HashPassword("Oper@123");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_Should_NotReturnEmpty()
    {
        var hash = SeedData.HashPassword("Admin@123");
        Assert.False(string.IsNullOrEmpty(hash));
    }
}
