using Folha360.Fiscais.Domain.Abstractions;

namespace Folha360.Fiscais.Domain.Services;

public class RegraFiscalFactory : IRegraFiscalFactory
{
    private readonly Dictionary<Tributo, IRegraFiscalService> _services;

    public RegraFiscalFactory(Dictionary<Tributo, IRegraFiscalService> services)
    {
        _services = services;
    }

    public IRegraFiscalService Resolver(Tributo tributo)
    {
        if (!_services.TryGetValue(tributo, out var service))
        {
            throw new InvalidOperationException($"Nenhum serviço de regra fiscal registrado para o tributo {tributo}.");
        }

        return service;
    }
}
