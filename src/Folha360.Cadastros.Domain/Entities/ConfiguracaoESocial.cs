using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

/// <summary>
/// Entidade ConfiguracaoESocial — configurações de e-Social da empresa (1:1).
/// Schema: tenant. Não implementa ISoftDeletable.
/// </summary>
public class ConfiguracaoESocial : BaseEntity
{
    public Guid EmpresaId { get; private set; }
    public string Ambiente { get; private set; } = null!;
    public string? CertificadoDigitalTipo { get; private set; }
    public DateOnly? CertificadoVencimento { get; private set; }
    public string? VersaoLayout { get; private set; }
    public string? CodigoTransmissor { get; private set; }
    public string? GrupoEsocial { get; private set; }
    public DateOnly? DataInicioObrigatoriedade { get; private set; }

    private ConfiguracaoESocial()
        : base()
    {
    }

    public ConfiguracaoESocial(
        Guid empresaId,
        string ambiente,
        string? certificadoDigitalTipo = null,
        DateOnly? certificadoVencimento = null,
        string? versaoLayout = null,
        string? codigoTransmissor = null,
        string? grupoEsocial = null,
        DateOnly? dataInicioObrigatoriedade = null)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Ambiente = ambiente;
        CertificadoDigitalTipo = certificadoDigitalTipo;
        CertificadoVencimento = certificadoVencimento;
        VersaoLayout = versaoLayout;
        CodigoTransmissor = codigoTransmissor;
        GrupoEsocial = grupoEsocial;
        DataInicioObrigatoriedade = dataInicioObrigatoriedade;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Atualizar(
        string ambiente,
        string? certificadoDigitalTipo = null,
        DateOnly? certificadoVencimento = null,
        string? versaoLayout = null,
        string? codigoTransmissor = null,
        string? grupoEsocial = null,
        DateOnly? dataInicioObrigatoriedade = null)
    {
        Ambiente = ambiente;
        CertificadoDigitalTipo = certificadoDigitalTipo;
        CertificadoVencimento = certificadoVencimento;
        VersaoLayout = versaoLayout;
        CodigoTransmissor = codigoTransmissor;
        GrupoEsocial = grupoEsocial;
        DataInicioObrigatoriedade = dataInicioObrigatoriedade;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se o certificado digital está próximo do vencimento (menos de 30 dias).
    /// </summary>
    public bool EstaProximoVencimento()
    {
        if (!CertificadoVencimento.HasValue)
        {
            return false;
        }

        var diasRestantes = CertificadoVencimento.Value.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow;
        return diasRestantes.TotalDays <= 30;
    }
}
