using Folha360.Domain;

namespace Folha360.Processamento.Domain.Entities;

public class ProcessamentoFolha : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public DateOnly Periodo { get; private set; }
    public TipoCalculo TipoCalculo { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public int Versao { get; private set; } = 1;
    public Guid? ProcessamentoOriginalId { get; private set; }
    public string? ReabertoPor { get; private set; }
    public DateTime? ReabertoEm { get; private set; }
    public string? MotivoReabertura { get; private set; }
    public int TotalFuncionarios { get; private set; }
    public int FuncionariosProcessados { get; private set; }
    public int FuncionariosComErro { get; private set; }
    public decimal TotalVencimentos { get; private set; }
    public decimal TotalDescontos { get; private set; }
    public decimal TotalLiquido { get; private set; }
    public decimal TotalFgts { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public string? Erro { get; private set; }

    private ProcessamentoFolha()
    {
    }

    public ProcessamentoFolha(Guid empresaId, DateOnly periodo, TipoCalculo tipoCalculo)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Periodo = periodo;
        TipoCalculo = tipoCalculo;
        Status = StatusProcessamento.Pendente;
        Versao = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ProcessamentoFolha(
        Guid empresaId,
        DateOnly periodo,
        TipoCalculo tipoCalculo,
        int versao,
        Guid processamentoOriginalId)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Periodo = periodo;
        TipoCalculo = tipoCalculo;
        Status = StatusProcessamento.Pendente;
        Versao = versao;
        ProcessamentoOriginalId = processamentoOriginalId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Iniciar(int totalFuncionarios)
    {
        Status = StatusProcessamento.EmProcessamento;
        TotalFuncionarios = totalFuncionarios;
        FuncionariosProcessados = 0;
        FuncionariosComErro = 0;
        DataInicio = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AtualizarProgresso(int processados, int comErro)
    {
        FuncionariosProcessados = processados;
        FuncionariosComErro = comErro;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Concluir(decimal totalVencimentos, decimal totalDescontos, decimal totalLiquido, decimal totalFgts)
    {
        Status = StatusProcessamento.Concluido;
        TotalVencimentos = totalVencimentos;
        TotalDescontos = totalDescontos;
        TotalLiquido = totalLiquido;
        TotalFgts = totalFgts;
        DataFim = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Falhar(string erro)
    {
        Status = StatusProcessamento.Falho;
        Erro = erro;
        DataFim = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Status = StatusProcessamento.Cancelado;
        DataFim = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reabrir(string reabertoPor, string motivo)
    {
        Status = StatusProcessamento.Reaberta;
        ReabertoPor = reabertoPor;
        ReabertoEm = DateTime.UtcNow;
        MotivoReabertura = motivo;
        UpdatedAt = DateTime.UtcNow;
    }
}
