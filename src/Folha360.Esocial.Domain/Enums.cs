namespace Folha360.Esocial.Domain;

public enum TipoEventoEsocial
{
    S1000,
    S1005,
    S1010,
    S1020,
    S1030,
    S1035,
    S1040,
    S1050,
    S1060,
    S1070,
    S1080,
    S1200,
    S1210,
    S1299,
    S2200,
    S2206,
    S2230,
    S2231,
    S2299,
    S2300,
    S2306,
    S2399,
    S2210,
    S2220,
    S2240,
    S5001,
    S5002,
}

public enum StatusEvento
{
    Pendente,
    Validado,
    Assinado,
    Enviado,
    Processado,
    Erro,
    Retificado,
}

public enum StatusLote
{
    Pendente,
    Assinando,
    Assinado,
    Enviado,
    Processado,
    Erro,
    ParcialmenteProcessado,
}

public enum TipoAmbiente
{
    Producao,
    Homologacao,
}

public enum TipoCertificado
{
    A1,
    A3,
}

public enum TipoErroEsocial
{
    Validacao,
    Assinatura,
    Envio,
    Processamento,
    Governo,
}
