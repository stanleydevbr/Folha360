using Folha360.Domain.Validation;

namespace Folha360.Cadastros.Domain.ValueObjects;

/// <summary>
/// Value Object imutável representando um endereço completo.
/// </summary>
public sealed record Endereco
{
    public string Logradouro { get; }
    public string Numero { get; }
    public string? Complemento { get; }
    public string Bairro { get; }
    public string Cep { get; }
    public string Municipio { get; }
    public string Uf { get; }

    private Endereco(
        string logradouro,
        string numero,
        string? complemento,
        string bairro,
        string cep,
        string municipio,
        string uf)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        Municipio = municipio;
        Uf = uf;
    }

    /// <summary>
    /// Cria um builder para construir um <see cref="Endereco"/> validado.
    /// </summary>
    public static EnderecoBuilder Create(
        string logradouro,
        string numero,
        string bairro,
        string cep,
        string municipio,
        string uf,
        string? complemento = null) => new(logradouro, numero, complemento, bairro, cep, municipio, uf);

    public override string ToString()
    {
        var complemento = string.IsNullOrEmpty(Complemento) ? string.Empty : $" - {Complemento}";
        return $"{Logradouro}, {Numero}{complemento}, {Bairro}, {Municipio}/{Uf}, CEP: {Cep}";
    }

    /// <summary>
    /// Builder para construção validada de <see cref="Endereco"/>.
    /// </summary>
    public sealed class EnderecoBuilder : NotifiableValueObject<Endereco>
    {
        private readonly string _logradouro;
        private readonly string _numero;
        private readonly string? _complemento;
        private readonly string _bairro;
        private readonly string _cep;
        private readonly string _municipio;
        private readonly string _uf;

        internal EnderecoBuilder(
            string logradouro,
            string numero,
            string? complemento,
            string bairro,
            string cep,
            string municipio,
            string uf)
        {
            _logradouro = logradouro;
            _numero = numero;
            _complemento = complemento;
            _bairro = bairro;
            _cep = cep;
            _municipio = municipio;
            _uf = uf;
        }

        public override Endereco? Build()
        {
            var valido = true;

            if (string.IsNullOrWhiteSpace(_logradouro))
            {
                Notification.AddError("ENDERECO_LOGRADOURO", "Logradouro é obrigatório.", nameof(_logradouro));
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_numero))
            {
                Notification.AddError("ENDERECO_NUMERO", "Número é obrigatório.", nameof(_numero));
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_bairro))
            {
                Notification.AddError("ENDERECO_BAIRRO", "Bairro é obrigatório.", nameof(_bairro));
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_cep))
            {
                Notification.AddError("ENDERECO_CEP", "CEP é obrigatório.", nameof(_cep));
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_municipio))
            {
                Notification.AddError("ENDERECO_MUNICIPIO", "Município é obrigatório.", nameof(_municipio));
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_uf) || _uf.Length != 2)
            {
                Notification.AddError("ENDERECO_UF", "UF deve ter 2 caracteres.", nameof(_uf));
                valido = false;
            }

            if (!valido)
                return null;

            return new Endereco(_logradouro, _numero, _complemento, _bairro, _cep, _municipio, _uf.ToUpperInvariant());
        }
    }
}
