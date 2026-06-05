namespace Folha360.Cadastros.Domain.Events;

public record EmpresaCadastradaEvent(
    Guid EmpresaId,
    string Cnpj,
    string RazaoSocial,
    string RegimeTributario,
    DateTime OcorridoEm)
{
    public EmpresaCadastradaEvent(Guid empresaId, string cnpj, string razaoSocial, string regimeTributario)
        : this(empresaId, cnpj, razaoSocial, regimeTributario, DateTime.UtcNow)
    {
    }
}
