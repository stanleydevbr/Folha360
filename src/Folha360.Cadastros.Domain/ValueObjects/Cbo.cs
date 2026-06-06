using System.Text.RegularExpressions;

namespace Folha360.Cadastros.Domain.ValueObjects;

/// <summary>
/// Value Object imutável representando um CBO (Classificação Brasileira de Ocupações).
/// Deve ter exatamente 6 dígitos numéricos, compatível com e-Social.
/// </summary>
public sealed record Cbo
{
    public string Codigo { get; }

    public Cbo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("CBO não pode ser vazio.", nameof(codigo));

        var apenasDigitos = Regex.Replace(codigo, @"[^\d]", string.Empty);

        if (apenasDigitos.Length != 6)
            throw new ArgumentException("CBO deve ter exatamente 6 dígitos numéricos.", nameof(codigo));

        Codigo = apenasDigitos;
    }

    public override string ToString() => Codigo;

    public static implicit operator string(Cbo cbo) => cbo.Codigo;
    public static explicit operator Cbo(string codigo) => new(codigo);
}
