using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Services;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class RegraFiscalFactoryTests
{
    [Fact]
    public void Resolver_DeveRetornarServicoCorreto_ParaCadaTributo()
    {
        var dict = new Dictionary<Tributo, IRegraFiscalService>
        {
            [Tributo.IRRF] = new IrpfRegraFiscalService(),
            [Tributo.INSS] = new InssRegraFiscalService(),
            [Tributo.FGTS] = new FgtsRegraFiscalService(),
            [Tributo.PIS] = new PisRegraFiscalService(),
            [Tributo.COFINS] = new CofinsRegraFiscalService(),
            [Tributo.CSLL] = new CsllRegraFiscalService(),
            [Tributo.ContribuicaoSindical] = new SindicalRegraFiscalService(),
            [Tributo.ISS] = new IssRegraFiscalService(),
        };
        var factory = new RegraFiscalFactory(dict);

        Assert.IsType<IrpfRegraFiscalService>(factory.Resolver(Tributo.IRRF));
        Assert.IsType<InssRegraFiscalService>(factory.Resolver(Tributo.INSS));
        Assert.IsType<FgtsRegraFiscalService>(factory.Resolver(Tributo.FGTS));
        Assert.IsType<PisRegraFiscalService>(factory.Resolver(Tributo.PIS));
        Assert.IsType<CofinsRegraFiscalService>(factory.Resolver(Tributo.COFINS));
        Assert.IsType<CsllRegraFiscalService>(factory.Resolver(Tributo.CSLL));
        Assert.IsType<SindicalRegraFiscalService>(factory.Resolver(Tributo.ContribuicaoSindical));
        Assert.IsType<IssRegraFiscalService>(factory.Resolver(Tributo.ISS));
    }

    [Fact]
    public void Resolver_DeveLancarExcecao_QuandoTributoNaoRegistrado()
    {
        var dict = new Dictionary<Tributo, IRegraFiscalService>
        {
            [Tributo.IRRF] = new IrpfRegraFiscalService(),
        };
        var factory = new RegraFiscalFactory(dict);

        Assert.Throws<InvalidOperationException>(() => factory.Resolver(Tributo.INSS));
    }
}
