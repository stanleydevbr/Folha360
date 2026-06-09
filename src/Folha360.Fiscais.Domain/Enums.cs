namespace Folha360.Fiscais.Domain;

public enum Tributo
{
    IRRF = 1,
    INSS = 2,
    FGTS = 3,
    ContribuicaoSindical = 4,
    PIS = 5,
    COFINS = 6,
    CSLL = 7,
    ISS = 8,
}

public enum StatusApuracao
{
    Pendente = 1,
    EmProcessamento = 2,
    Concluido = 3,
    Falho = 4,
    Revertido = 5,
}

public enum TipoGuia
{
    GPS = 1,
    DARF = 2,
    GRF = 3,
}

public enum StatusGuia
{
    Pendente = 1,
    Gerada = 2,
    Paga = 3,
    Vencida = 4,
    Cancelada = 5,
}

public enum FormatoExportacao
{
    CSV = 1,
    SPED_ECD = 2,
    SPED_ECF = 3,
}
