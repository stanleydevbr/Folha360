using Folha360.Domain;
using Folha360.Domain.Attributes;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade RemuneracaoBeneficio — remuneração e benefícios do funcionário (1:1).
/// Schema: tenant.
/// </summary>
public class RemuneracaoBeneficio : BaseEntity
{
    public Guid FuncionarioId { get; private set; }
    public Guid? ConvenioId { get; private set; }

    [SensitiveData]
    public decimal SalarioBase { get; private set; }

    public decimal? ValorHora { get; private set; }
    public decimal? AdicionalInsalubridade { get; private set; }
    public decimal? AdicionalPericulosidade { get; private set; }
    public decimal? AdicionalNoturnoPercentual { get; private set; }
    public decimal? AdicionalTransferenciaPercentual { get; private set; }
    public bool ValeTransporte { get; private set; }
    public decimal? ValeTransporteValor { get; private set; }
    public bool ValeRefeicao { get; private set; }
    public decimal? ValeRefeicaoValorDiario { get; private set; }
    public bool PlanoSaude { get; private set; }
    public decimal? PlanoSaudeValor { get; private set; }
    public bool PlanoOdontologico { get; private set; }
    public decimal? PlanoOdontologicoValor { get; private set; }
    public bool SeguroVida { get; private set; }
    public decimal? SeguroVidaValor { get; private set; }
    public bool PrevidenciaPrivada { get; private set; }
    public decimal? PrevidenciaPrivadaValor { get; private set; }

    private RemuneracaoBeneficio()
        : base()
    {
    }

    public RemuneracaoBeneficio(
        Guid funcionarioId,
        decimal salarioBase,
        Guid? convenioId = null,
        decimal? valorHora = null,
        decimal? adicionalInsalubridade = null,
        decimal? adicionalPericulosidade = null,
        decimal? adicionalNoturnoPercentual = null,
        decimal? adicionalTransferenciaPercentual = null,
        bool valeTransporte = false,
        decimal? valeTransporteValor = null,
        bool valeRefeicao = false,
        decimal? valeRefeicaoValorDiario = null,
        bool planoSaude = false,
        decimal? planoSaudeValor = null,
        bool planoOdontologico = false,
        decimal? planoOdontologicoValor = null,
        bool seguroVida = false,
        decimal? seguroVidaValor = null,
        bool previdenciaPrivada = false,
        decimal? previdenciaPrivadaValor = null)
    {
        Id = Guid.NewGuid();
        FuncionarioId = funcionarioId;
        SalarioBase = salarioBase;
        ConvenioId = convenioId;
        ValorHora = valorHora;
        AdicionalInsalubridade = adicionalInsalubridade;
        AdicionalPericulosidade = adicionalPericulosidade;
        AdicionalNoturnoPercentual = adicionalNoturnoPercentual;
        AdicionalTransferenciaPercentual = adicionalTransferenciaPercentual;
        ValeTransporte = valeTransporte;
        ValeTransporteValor = valeTransporteValor;
        ValeRefeicao = valeRefeicao;
        ValeRefeicaoValorDiario = valeRefeicaoValorDiario;
        PlanoSaude = planoSaude;
        PlanoSaudeValor = planoSaudeValor;
        PlanoOdontologico = planoOdontologico;
        PlanoOdontologicoValor = planoOdontologicoValor;
        SeguroVida = seguroVida;
        SeguroVidaValor = seguroVidaValor;
        PrevidenciaPrivada = previdenciaPrivada;
        PrevidenciaPrivadaValor = previdenciaPrivadaValor;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        decimal salarioBase,
        decimal? valorHora = null,
        decimal? adicionalInsalubridade = null,
        decimal? adicionalPericulosidade = null,
        decimal? adicionalNoturnoPercentual = null,
        decimal? adicionalTransferenciaPercentual = null,
        bool? valeTransporte = null,
        decimal? valeTransporteValor = null,
        bool? valeRefeicao = null,
        decimal? valeRefeicaoValorDiario = null,
        bool? planoSaude = null,
        decimal? planoSaudeValor = null,
        bool? planoOdontologico = null,
        decimal? planoOdontologicoValor = null,
        bool? seguroVida = null,
        decimal? seguroVidaValor = null,
        bool? previdenciaPrivada = null,
        decimal? previdenciaPrivadaValor = null)
    {
        SalarioBase = salarioBase;
        ValorHora = valorHora;
        AdicionalInsalubridade = adicionalInsalubridade;
        AdicionalPericulosidade = adicionalPericulosidade;
        AdicionalNoturnoPercentual = adicionalNoturnoPercentual;
        AdicionalTransferenciaPercentual = adicionalTransferenciaPercentual;

        if (valeTransporte.HasValue)
        {
            ValeTransporte = valeTransporte.Value;
        }

        ValeTransporteValor = valeTransporteValor;

        if (valeRefeicao.HasValue)
        {
            ValeRefeicao = valeRefeicao.Value;
        }

        ValeRefeicaoValorDiario = valeRefeicaoValorDiario;

        if (planoSaude.HasValue)
        {
            PlanoSaude = planoSaude.Value;
        }

        PlanoSaudeValor = planoSaudeValor;

        if (planoOdontologico.HasValue)
        {
            PlanoOdontologico = planoOdontologico.Value;
        }

        PlanoOdontologicoValor = planoOdontologicoValor;

        if (seguroVida.HasValue)
        {
            SeguroVida = seguroVida.Value;
        }

        SeguroVidaValor = seguroVidaValor;

        if (previdenciaPrivada.HasValue)
        {
            PrevidenciaPrivada = previdenciaPrivada.Value;
        }

        PrevidenciaPrivadaValor = previdenciaPrivadaValor;

        UpdatedAt = DateTime.UtcNow;
    }
}
