using Folha360.Domain;

namespace Folha360.Processamento.Domain.Entities;

public class CadeiaFechamento : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public DateOnly Periodo { get; private set; }
    public Guid ProcessamentoId { get; private set; }
    public EtapaFechamento Etapa { get; private set; }
    public StatusEtapa Status { get; private set; }
    public int Versao { get; private set; } = 1;
    public string HistoricoVersoes { get; private set; } = "[]";
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public string? Erro { get; private set; }

    private CadeiaFechamento()
    {
    }

    public CadeiaFechamento(Guid empresaId, DateOnly periodo, Guid processamentoId)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Periodo = periodo;
        ProcessamentoId = processamentoId;
        Etapa = EtapaFechamento.FolhaProcessada;
        Status = StatusEtapa.Pendente;
        Versao = 1;
        HistoricoVersoes = "[]";
        DataInicio = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AvancarEtapa(EtapaFechamento novaEtapa)
    {
        Etapa = novaEtapa;
        Status = StatusEtapa.Concluido;
        UpdatedAt = DateTime.UtcNow;

        if (novaEtapa == EtapaFechamento.FechamentoConcluido)
        {
            DataFim = DateTime.UtcNow;
        }
    }

    public void FalharEtapa(string erro)
    {
        Status = StatusEtapa.Falhou;
        Erro = erro;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Compensar()
    {
        Status = StatusEtapa.Compensando;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reabrir(string historico)
    {
        Status = StatusEtapa.Reaberta;
        Versao++;
        HistoricoVersoes = historico;
        UpdatedAt = DateTime.UtcNow;
    }
}
