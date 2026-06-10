namespace Folha360.Fiscais.Domain.Abstractions;

public interface IRegraFiscalFactory
{
    IRegraFiscalService Resolver(Tributo tributo);
}
