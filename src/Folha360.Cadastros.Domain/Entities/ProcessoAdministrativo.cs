using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class ProcessoAdministrativo : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string NumeroProcesso { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;
    public string? Orgao { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public string? Observacao { get; private set; }
    public ICollection<RubricaProcesso> RubricasProcesso { get; private set; } = new List<RubricaProcesso>();

    private ProcessoAdministrativo()
    {
    }

    public ProcessoAdministrativo(Guid empresaId, string numeroProcesso, string tipo, string? orgao = null, DateTime? dataInicio = null, DateTime? dataFim = null, string? observacao = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        NumeroProcesso = numeroProcesso;
        Tipo = tipo;
        Orgao = orgao;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Observacao = observacao;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(string? numeroProcesso = null, string? tipo = null, string? orgao = null, DateTime? dataInicio = null, DateTime? dataFim = null, string? observacao = null)
    {
        NumeroProcesso = numeroProcesso ?? NumeroProcesso;
        Tipo = tipo ?? Tipo;
        Orgao = orgao ?? Orgao;
        DataInicio = dataInicio ?? DataInicio;
        DataFim = dataFim ?? DataFim;
        Observacao = observacao ?? Observacao;
        UpdatedAt = DateTime.UtcNow;
    }
}
