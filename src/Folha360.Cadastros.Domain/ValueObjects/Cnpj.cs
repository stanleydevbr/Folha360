using System.Text.RegularExpressions;

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

    public Cnpj(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CNPJ não pode ser vazio.", nameof(numero));

        var apenasDigitos = Regex.Replace(numero, @"[^\d]", string.Empty);

        if (apenasDigitos.Length != 14)
            throw new ArgumentException("CNPJ deve ter 14 dígitos.", nameof(numero));

        if (!ValidarDigitosVerificadores(apenasDigitos))
            throw new ArgumentException("CNPJ inválido — dígitos verificadores não conferem.", nameof(numero));

        Numero = apenasDigitos;
    }

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
    public static explicit operator Cnpj(string numero) => new(numero);
}
