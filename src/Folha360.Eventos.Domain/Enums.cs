namespace Folha360.Eventos.Domain;

public enum TipoContrato
{
    Indeterminado = 0,
    Determinado = 1,
    Experiencia = 2,
    Temporario = 3,
    Intermitente = 4,
}

public enum TipoFerias
{
    Normais = 0,
    Coletivas = 1,
    Antecipadas = 2,
    Dobro = 3,
}

public enum TipoAfastamento
{
    Doenca = 0,
    AcidenteTrabalho = 1,
    LicencaMaternidade = 2,
    LicencaPaternidade = 3,
    ServicoMilitar = 4,
    SuspensaoContratual = 5,
}

public enum MotivoDesligamento
{
    SemJustaCausa = 0,
    ComJustaCausa = 1,
    PedidoDemissao = 2,
    TerminoContrato = 3,
    AcordoMutuo = 4,
    Aposentadoria = 5,
    Morte = 6,
}
