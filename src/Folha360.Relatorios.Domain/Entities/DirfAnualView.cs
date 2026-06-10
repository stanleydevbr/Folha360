namespace Folha360.Relatorios.Domain.Entities;

public class DirfAnualView
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
    public Guid FuncionarioId { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string NomeFuncionario { get; set; } = string.Empty;
    public decimal RendimentosTributaveis { get; set; }
    public decimal RendimentosIsentos { get; set; }
    public decimal IrrfRetido { get; set; }
    public decimal DecimoTerceiro { get; set; }
    public decimal Ferias { get; set; }
}
