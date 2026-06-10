namespace Folha360.Relatorios.Domain.Entities;

public class RaisAnualView
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
    public Guid FuncionarioId { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string NomeFuncionario { get; set; } = string.Empty;
    public string PisPasep { get; set; } = string.Empty;
    public DateTime? DataAdmissao { get; set; }
    public DateTime? DataDesligamento { get; set; }
    public string MotivoDesligamento { get; set; } = string.Empty;
    public decimal RemuneracaoJaneiro { get; set; }
    public decimal RemuneracaoFevereiro { get; set; }
    public decimal RemuneracaoMarco { get; set; }
    public decimal RemuneracaoAbril { get; set; }
    public decimal RemuneracaoMaio { get; set; }
    public decimal RemuneracaoJunho { get; set; }
    public decimal RemuneracaoJulho { get; set; }
    public decimal RemuneracaoAgosto { get; set; }
    public decimal RemuneracaoSetembro { get; set; }
    public decimal RemuneracaoOutubro { get; set; }
    public decimal RemuneracaoNovembro { get; set; }
    public decimal RemuneracaoDezembro { get; set; }
    public decimal RemuneracaoTotal { get; set; }
    public decimal DecimoTerceiro { get; set; }
}
