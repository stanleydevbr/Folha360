using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Folha360.Cadastros.Infrastructure.Data;

/// <summary>
/// ValueConverter that encrypts sensitive data using ASP.NET Core Data Protection (AES-256-GCM).
/// On write: encrypts the plain text value.
/// On read: decrypts the cipher text value.
/// </summary>
public class EncryptionConverter : ValueConverter<string, string>
{
    private static readonly ILoggerFactory FallbackLoggerFactory =
        LoggerFactory.Create(builder => builder.AddConsole());

    /// <summary>
    /// Creates an EncryptionConverter with a Base64 fallback for development/testing only.
    /// Throws InvalidOperationException in Production environment.
    /// </summary>
    public EncryptionConverter()
        : this(CreateDevProtector())
    {
    }

    /// <summary>
    /// Creates an EncryptionConverter using ASP.NET Core Data Protection (AES-256-GCM).
    /// </summary>
    public EncryptionConverter(Microsoft.AspNetCore.DataProtection.IDataProtector protector)
        : base(
            plain => Convert.ToBase64String(protector.Protect(System.Text.Encoding.UTF8.GetBytes(plain))),
            cipher => DecryptWithProtector(protector, cipher))
    {
    }

    private static Microsoft.AspNetCore.DataProtection.IDataProtector CreateDevProtector()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.Equals(env, "Production", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Data Protection must be configured in Production. " +
                "The parameterless EncryptionConverter constructor uses Base64 fallback " +
                "and is not secure for production use.");
        }

        var logger = FallbackLoggerFactory.CreateLogger<EncryptionConverter>();
        logger.LogWarning(
            "EncryptionConverter using Base64 fallback — NOT SECURE for production. " +
            "Configure ASP.NET Core Data Protection for AES-256-GCM encryption.");

        return new Base64Protector();
    }

    private static string DecryptWithProtector(
        Microsoft.AspNetCore.DataProtection.IDataProtector protector, string cipher)
    {
        try
        {
            var bytes = protector.Unprotect(Convert.FromBase64String(cipher));
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (Exception ex)
        {
            var logger = FallbackLoggerFactory.CreateLogger<EncryptionConverter>();
            logger.LogError(ex, "Failed to decrypt sensitive data. Data may be corrupted or the protection key has changed.");
            throw new InvalidOperationException(
                "Failed to decrypt sensitive data. Verify Data Protection key configuration.", ex);
        }
    }

    /// <summary>
    /// Development-only protector using Base64 encoding. Not cryptographically secure.
    /// </summary>
    private sealed class Base64Protector : Microsoft.AspNetCore.DataProtection.IDataProtector
    {
        public byte[] Protect(byte[] plaintext)
            => System.Text.Encoding.UTF8.GetBytes(Convert.ToBase64String(plaintext));

        public byte[] Unprotect(byte[] protectedData)
            => Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(protectedData));

        public Microsoft.AspNetCore.DataProtection.IDataProtector CreateProtector(string purpose)
            => this;
    }
}
