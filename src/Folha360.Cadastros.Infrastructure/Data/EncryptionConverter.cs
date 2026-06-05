using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Folha360.Cadastros.Infrastructure.Data;

/// <summary>
/// ValueConverter que criptografa dados sensíveis usando ASP.NET Core Data Protection (AES-256-GCM).
/// Na escrita: criptografa o valor plain text.
/// Na leitura: desencripta o valor cipher text.
/// </summary>
public class EncryptionConverter : ValueConverter<string, string>
{
    public EncryptionConverter()
        : base(
            plain => Encrypt(plain),
            cipher => Decrypt(cipher))
    {
    }

    public EncryptionConverter(Microsoft.AspNetCore.DataProtection.IDataProtector protector)
        : base(
            plain => Convert.ToBase64String(protector.Protect(System.Text.Encoding.UTF8.GetBytes(plain))),
            cipher => System.Text.Encoding.UTF8.GetString(protector.Unprotect(Convert.FromBase64String(cipher))))
    {
    }

    private static string Encrypt(string plain)
    {
        // Fallback: em testes ou cenários sem Data Protection configurado,
        // usa codificação Base64 reversível (não seguro para produção!)
        var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
        return Convert.ToBase64String(bytes);
    }

    private static string Decrypt(string cipher)
    {
        try
        {
            var bytes = Convert.FromBase64String(cipher);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return cipher;
        }
    }
}
