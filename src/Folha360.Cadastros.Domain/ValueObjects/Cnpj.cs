using System.Text.RegularExpressions;
using Folha360.Domain.Validation;

namespace Folha360.Cadastros.Domain.ValueObjects;

/// <summary>
/// Value Object imutável representando um CNPJ validado.
/// Encapsula validação de dígitos verificadores e formatação.
/// </summary>
public sealed record Cnpj
{
    private static readonly int[] Multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] Multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    public string Numero { get; }
    public string Formatado => Convert.ToUInt64(Numero).ToString(@"00\.000\.000\/0000\-00");

    private Cnpj(string numero)
    {
        Numero = numero;
    }

    /// <summary>
    /// Cria um builder para construir um <see cref="Cnpj"/> validado.
    /// </summary>
    public static CnpjBuilder Create(string numero) => new(numero);

    private static bool ValidarDigitosVerificadores(string cnpj)
    {
        if (new string(cnpj[0], 14) == cnpj)
            return false;

        var digito1 = CalcularDigito(cnpj, Multiplicadores1);
        var digito2 = CalcularDigito(cnpj, Multiplicadores2);

        return cnpj[12] - '0' == digito1 && cnpj[13] - '0' == digito2;
    }

    private static int CalcularDigito(string cnpj, int[] multiplicadores)
    {
        var soma = 0;
        for (var i = 0; i < multiplicadores.Length; i++)
            soma += (cnpj[i] - '0') * multiplicadores[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public override string ToString() => Formatado;

    public static implicit operator string(Cnpj cnpj) => cnpj.Numero;

    /// <summary>
    /// Builder para construção validada de <see cref="Cnpj"/>.
    /// </summary>
    public sealed class CnpjBuilder : NotifiableValueObject<Cnpj>
    {
        private readonly string _numeroOriginal;

        internal CnpjBuilder(string numero)
        {
            _numeroOriginal = numero;
        }

        public override Cnpj? Build()
        {
            if (string.IsNullOrWhiteSpace(_numeroOriginal))
            {
                Notification.AddError("CNPJ_VAZIO", "CNPJ não pode ser vazio.");
                return null;
            }

            var apenasDigitos = Regex.Replace(_numeroOriginal, @"[^\d]", string.Empty);

            if (apenasDigitos.Length != 14)
            {
                Notification.AddError("CNPJ_TAMANHO", "CNPJ deve ter 14 dígitos.", nameof(Numero));
                return null;
            }

            if (!ValidarDigitosVerificadores(apenasDigitos))
            {
                Notification.AddError("CNPJ_INVALIDO", "CNPJ inválido — dígitos verificadores não conferem.", nameof(Numero));
                return null;
            }

            return new Cnpj(apenasDigitos);
        }
    }
}
