using Folha360.Domain;

namespace Folha360.Fiscais.Domain.Entities;

public class RegraFiscal : BaseEntity
{
    public Tributo Tributo { get; private set; }
    public int Versao { get; private set; }
    public DateOnly VigenciaInicio { get; private set; }
    public DateOnly? VigenciaFim { get; private set; }
    public string Parametros { get; private set; } = "{}";
    public string CodigoReceita { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;

    private RegraFiscal()
    {
    }

    public RegraFiscal(
        Tributo tributo,
        int versao,
        DateOnly vigenciaInicio,
        DateOnly? vigenciaFim,
        string parametros,
        string codigoReceita)
    {
        Tributo = tributo;
        Versao = versao;
        VigenciaInicio = vigenciaInicio;
        VigenciaFim = vigenciaFim;
        Parametros = parametros;
        CodigoReceita = codigoReceita;
        Ativo = true;
    }

    public void Ativar()
    {
        Ativo = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desativar()
    {
        Ativo = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool EstaVigente(DateOnly data)
    {
        return Ativo
            && data >= VigenciaInicio
            && (VigenciaFim == null || data <= VigenciaFim);
    }
}
