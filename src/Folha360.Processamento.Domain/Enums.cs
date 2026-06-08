namespace Folha360.Processamento.Domain;

public enum TipoCalculo
{
    Mensal = 0,
    Ferias = 1,
    DecimoTerceiro = 2,
    Rescisao = 3,
    Dissidio = 4,
    Complementar = 5,
    AuxilioDoenca = 6,
    SalarioMaternidade = 7,
    Acordo = 8,
    Estagio = 9,
    Rpa = 10,
}

public enum FaseProcessamento
{
    Vencimentos = 1,
    Bases = 2,
    Descontos = 3,
    Totais = 4,
}

public enum StatusProcessamento
{
    Pendente = 0,
    EmProcessamento = 1,
    Concluido = 2,
    Falho = 3,
    Cancelado = 4,
    Reaberta = 5,
}

public enum EtapaFechamento
{
    FolhaProcessada = 0,
    ObrigacoesApuradas = 1,
    EventosESocialEnviados = 2,
    FechamentoConcluido = 3,
}

public enum StatusEtapa
{
    Pendente = 0,
    EmProgresso = 1,
    Concluido = 2,
    Falhou = 3,
    Compensando = 4,
    Reaberta = 5,
}
