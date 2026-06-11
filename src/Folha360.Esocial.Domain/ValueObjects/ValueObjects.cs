namespace Folha360.Esocial.Domain.ValueObjects;

public sealed record ProtocoloEnvio
{
    public string Numero { get; }

    public ProtocoloEnvio(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número do protocolo não pode ser vazio.", nameof(numero));
        Numero = numero;
    }

    public override string ToString() => Numero;
}

public sealed record ReciboGoverno
{
    public string Numero { get; }
    public DateTime Data { get; }
    public string Hash { get; }

    public ReciboGoverno(string numero, DateTime data, string hash)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número do recibo não pode ser vazio.", nameof(numero));
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash do recibo não pode ser vazio.", nameof(hash));
        Numero = numero;
        Data = data;
        Hash = hash;
    }
}
