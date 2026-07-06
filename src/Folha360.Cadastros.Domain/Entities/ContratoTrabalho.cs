using Folha360.Domain;
using Folha360.Domain.Attributes;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade ContratoTrabalho — vínculo empregatício detalhado (1:1 com Funcionario).
/// Schema: tenant.
/// </summary>
public class ContratoTrabalho : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid EmpresaId { get; private set; }
    public Guid? LotacaoId { get; private set; }
    public Guid? CargoId { get; private set; }
    public Guid? HorarioTrabalhoId { get; private set; }
    public Guid? SindicatoId { get; private set; }
    public DateOnly DataAdmissao { get; private set; }
    public DateOnly? DataDesligamento { get; private set; }
    public string? TipoAdmissao { get; private set; }
    public string TipoContrato { get; private set; } = null!;
    public DateOnly? DataTerminoContrato { get; private set; }

    [SensitiveData]
    public decimal SalarioBase { get; private set; }

    public string TipoSalario { get; private set; } = null!;
    public int? CargaHorariaSemanal { get; private set; }
    public string? CategoriaTrabalhador { get; private set; }
    public string? IndicativoAdmissao { get; private set; }
    public string Status { get; private set; } = "ATIVO";

    private ContratoTrabalho()
        : base()
    {
    }

    public ContratoTrabalho(
        Guid funcionarioId,
        Guid empresaId,
        DateOnly dataAdmissao,
        string tipoContrato,
        decimal salarioBase,
        string tipoSalario,
        Guid? lotacaoId = null,
        Guid? cargoId = null,
        Guid? horarioTrabalhoId = null,
        Guid? sindicatoId = null,
        DateOnly? dataDesligamento = null,
        string? tipoAdmissao = null,
        DateOnly? dataTerminoContrato = null,
        int? cargaHorariaSemanal = null,
        string? categoriaTrabalhador = null,
        string? indicativoAdmissao = null)
    {
        if (tipoContrato is "CLT_DETERMINADO" or "CLT_EXPERIENCIA" or "TEMPORARIO"
            && !dataTerminoContrato.HasValue)
        {
            throw new ArgumentException("Data de término é obrigatória para contrato por prazo determinado.");
        }

        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        EmpresaId = empresaId;
        DataAdmissao = dataAdmissao;
        TipoContrato = tipoContrato;
        SalarioBase = salarioBase;
        TipoSalario = tipoSalario;
        LotacaoId = lotacaoId;
        CargoId = cargoId;
        HorarioTrabalhoId = horarioTrabalhoId;
        SindicatoId = sindicatoId;
        DataDesligamento = dataDesligamento;
        TipoAdmissao = tipoAdmissao;
        DataTerminoContrato = dataTerminoContrato;
        CargaHorariaSemanal = cargaHorariaSemanal;
        CategoriaTrabalhador = categoriaTrabalhador;
        IndicativoAdmissao = indicativoAdmissao;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desligar(DateOnly dataDesligamento)
    {
        DataDesligamento = dataDesligamento;
        Status = "DESLIGADO";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        decimal salarioBase,
        string tipoSalario,
        Guid? lotacaoId = null,
        Guid? cargoId = null,
        Guid? horarioTrabalhoId = null,
        Guid? sindicatoId = null,
        int? cargaHorariaSemanal = null,
        string? categoriaTrabalhador = null)
    {
        SalarioBase = salarioBase;
        TipoSalario = tipoSalario;
        LotacaoId = lotacaoId;
        CargoId = cargoId;
        HorarioTrabalhoId = horarioTrabalhoId;
        SindicatoId = sindicatoId;
        CargaHorariaSemanal = cargaHorariaSemanal;
        CategoriaTrabalhador = categoriaTrabalhador;
        UpdatedAt = DateTime.UtcNow;
    }
}
