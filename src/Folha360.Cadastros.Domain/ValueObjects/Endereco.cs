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

    public Endereco(
        string logradouro,
        string numero,
        string bairro,
        string cep,
        string municipio,
        string uf,
        string? complemento = null)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro é obrigatório.", nameof(logradouro));
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório.", nameof(numero));
        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório.", nameof(bairro));
        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP é obrigatório.", nameof(cep));
        if (string.IsNullOrWhiteSpace(municipio))
            throw new ArgumentException("Município é obrigatório.", nameof(municipio));
        if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
            throw new ArgumentException("UF deve ter 2 caracteres.", nameof(uf));

        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        Municipio = municipio;
        Uf = uf.ToUpperInvariant();
    }

    public override string ToString()
    {
        var complemento = string.IsNullOrEmpty(Complemento) ? string.Empty : $" - {Complemento}";
        return $"{Logradouro}, {Numero}{complemento}, {Bairro}, {Municipio}/{Uf}, CEP: {Cep}";
    }
}
