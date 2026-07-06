using Folha360.Domain.Validation;

namespace Folha360.Esocial.Domain.ValueObjects;

public sealed record ProtocoloEnvio
{
    public string Numero { get; }

    private ProtocoloEnvio(string numero)
    {
        Numero = numero;
    }

    public static ProtocoloEnvioBuilder Create(string numero) => new(numero);

    public override string ToString() => Numero;

    public sealed class ProtocoloEnvioBuilder : NotifiableValueObject<ProtocoloEnvio>
    {
        private readonly string _numero;

        internal ProtocoloEnvioBuilder(string numero)
        {
            _numero = numero;
        }

        public override ProtocoloEnvio? Build()
        {
            if (string.IsNullOrWhiteSpace(_numero))
            {
                Notification.AddError("PROTOCOLO_VAZIO", "Número do protocolo não pode ser vazio.");
                return null;
            }

            return new ProtocoloEnvio(_numero);
        }
    }
}

public sealed record ReciboGoverno
{
    public string Numero { get; }
    public DateTime Data { get; }
    public string Hash { get; }

    private ReciboGoverno(string numero, DateTime data, string hash)
    {
        Numero = numero;
        Data = data;
        Hash = hash;
    }

    public static ReciboGovernoBuilder Create(string numero, DateTime data, string hash) => new(numero, data, hash);

    public sealed class ReciboGovernoBuilder : NotifiableValueObject<ReciboGoverno>
    {
        private readonly string _numero;
        private readonly DateTime _data;
        private readonly string _hash;

        internal ReciboGovernoBuilder(string numero, DateTime data, string hash)
        {
            _numero = numero;
            _data = data;
            _hash = hash;
        }

        public override ReciboGoverno? Build()
        {
            var valido = true;

            if (string.IsNullOrWhiteSpace(_numero))
            {
                Notification.AddError("RECIBO_VAZIO", "Número do recibo não pode ser vazio.");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(_hash))
            {
                Notification.AddError("RECIBO_HASH_VAZIO", "Hash do recibo não pode ser vazio.");
                valido = false;
            }

            if (!valido)
                return null;

            return new ReciboGoverno(_numero, _data, _hash);
        }
    }
}
