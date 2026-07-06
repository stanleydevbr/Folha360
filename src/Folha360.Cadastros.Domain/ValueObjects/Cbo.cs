using System.Text.RegularExpressions;
using Folha360.Domain.Validation;

namespace Folha360.Cadastros.Domain.ValueObjects;

/// <summary>
/// Value Object imutável representando um CBO (Classificação Brasileira de Ocupações).
/// Deve ter exatamente 6 dígitos numéricos, compatível com e-Social.
/// </summary>
public sealed record Cbo
{
    public string Codigo { get; }

    private Cbo(string codigo)
    {
        Codigo = codigo;
    }

    /// <summary>
    /// Cria um builder para construir um <see cref="Cbo"/> validado.
    /// </summary>
    public static CboBuilder Create(string codigo) => new(codigo);

    public override string ToString() => Codigo;

    public static implicit operator string(Cbo cbo) => cbo.Codigo;

    /// <summary>
    /// Builder para construção validada de <see cref="Cbo"/>.
    /// </summary>
    public sealed class CboBuilder : NotifiableValueObject<Cbo>
    {
        private readonly string _codigoOriginal;

        internal CboBuilder(string codigo)
        {
            _codigoOriginal = codigo;
        }

        public override Cbo? Build()
        {
            if (string.IsNullOrWhiteSpace(_codigoOriginal))
            {
                Notification.AddError("CBO_VAZIO", "CBO não pode ser vazio.");
                return null;
            }

            var apenasDigitos = Regex.Replace(_codigoOriginal, @"[^\d]", string.Empty);

            if (apenasDigitos.Length != 6)
            {
                Notification.AddError("CBO_TAMANHO", "CBO deve ter exatamente 6 dígitos numéricos.", nameof(Codigo));
                return null;
            }

            return new Cbo(apenasDigitos);
        }
    }
}
