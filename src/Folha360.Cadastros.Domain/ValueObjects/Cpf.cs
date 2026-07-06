using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Folha360.Domain.Validation;

namespace Folha360.Cadastros.Domain.ValueObjects;

/// <summary>
/// Value Object imutável representando um CPF validado.
/// Encapsula validação de dígitos verificadores e geração de hash SHA-256 para busca indexada.
/// </summary>
public sealed record Cpf
{
    private static readonly int[] Multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] Multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    public string Numero { get; }
    public string Formatado => Convert.ToUInt64(Numero).ToString(@"000\.000\.000\-00");
    public string Hash => ComputeHash();

    private Cpf(string numero)
    {
        Numero = numero;
    }

    /// <summary>
    /// Cria um builder para construir um <see cref="Cpf"/> validado.
    /// </summary>
    public static CpfBuilder Create(string numero) => new(numero);

    /// <summary>
    /// Calcula o hash SHA-256 do CPF para busca exata indexada.
    /// </summary>
    public string ComputeHash()
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(Numero));
        return Convert.ToHexStringLower(bytes);
    }

    private static bool ValidarDigitosVerificadores(string cpf)
    {
        if (new string(cpf[0], 11) == cpf)
            return false;

        var digito1 = CalcularDigito(cpf, Multiplicadores1);
        var digito2 = CalcularDigito(cpf, Multiplicadores2);

        return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
    }

    private static int CalcularDigito(string cpf, int[] multiplicadores)
    {
        var soma = 0;
        for (var i = 0; i < multiplicadores.Length; i++)
            soma += (cpf[i] - '0') * multiplicadores[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() => Formatado;

    public static implicit operator string(Cpf cpf) => cpf.Numero;

    /// <summary>
    /// Builder para construção validada de <see cref="Cpf"/>.
    /// </summary>
    public sealed class CpfBuilder : NotifiableValueObject<Cpf>
    {
        private readonly string _numeroOriginal;

        internal CpfBuilder(string numero)
        {
            _numeroOriginal = numero;
        }

        public override Cpf? Build()
        {
            if (string.IsNullOrWhiteSpace(_numeroOriginal))
            {
                Notification.AddError("CPF_VAZIO", "CPF não pode ser vazio.");
                return null;
            }

            var apenasDigitos = Regex.Replace(_numeroOriginal, @"[^\d]", string.Empty);

            if (apenasDigitos.Length != 11)
            {
                Notification.AddError("CPF_TAMANHO", "CPF deve ter 11 dígitos.", nameof(Numero));
                return null;
            }

            if (!ValidarDigitosVerificadores(apenasDigitos))
            {
                Notification.AddError("CPF_INVALIDO", "CPF inválido — dígitos verificadores não conferem.", nameof(Numero));
                return null;
            }

            return new Cpf(apenasDigitos);
        }
    }
}
