using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Entities;
using Xunit;

namespace Folha360.Tests.Fiscais.Domain;

public class GuiaRecolhimentoTests
{
    [Fact]
    public void Construtor_DeveCriarGuiaComStatusPendente()
    {
        var guia = new GuiaRecolhimento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            TipoGuia.GPS,
            Tributo.INSS,
            "2100",
            5000m,
            new DateOnly(2026, 7, 20));

        Assert.Equal(StatusGuia.Pendente, guia.Status);
        Assert.Equal(5000m, guia.Valor);
    }

    [Fact]
    public void Gerar_DeveAtualizarMinioKeyEStatus()
    {
        var guia = new GuiaRecolhimento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            TipoGuia.DARF,
            Tributo.IRRF,
            "0561",
            3000m,
            new DateOnly(2026, 7, 20));

        guia.Gerar("empresa-1/2026-06/darf_irrf.pdf");

        Assert.Equal(StatusGuia.Gerada, guia.Status);
        Assert.Equal("empresa-1/2026-06/darf_irrf.pdf", guia.MinioKey);
    }

    [Fact]
    public void RegistrarPagamento_DeveAtualizarStatusParaPaga()
    {
        var guia = new GuiaRecolhimento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            TipoGuia.GRF,
            Tributo.FGTS,
            "115",
            800m,
            new DateOnly(2026, 7, 7));
        guia.Gerar("key.pdf");

        guia.RegistrarPagamento(800m, new DateTime(2026, 7, 5));

        Assert.Equal(StatusGuia.Paga, guia.Status);
        Assert.Equal(800m, guia.ValorPago);
        Assert.Equal(new DateTime(2026, 7, 5), guia.DataPagamento);
    }

    [Fact]
    public void Cancelar_DeveAtualizarStatusParaCancelada()
    {
        var guia = new GuiaRecolhimento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            TipoGuia.GPS,
            Tributo.INSS,
            "2100",
            5000m,
            new DateOnly(2026, 7, 20));
        guia.Gerar("key.pdf");

        guia.Cancelar();

        Assert.Equal(StatusGuia.Cancelada, guia.Status);
    }

    [Fact]
    public void Vencer_DeveAtualizarStatusParaVencida_ApenasSeGerada()
    {
        var guia = new GuiaRecolhimento(
            Guid.NewGuid(),
            new DateOnly(2026, 6, 1),
            Guid.NewGuid(),
            TipoGuia.GPS,
            Tributo.INSS,
            "2100",
            5000m,
            new DateOnly(2026, 7, 20));
        guia.Gerar("key.pdf");

        guia.Vencer();

        Assert.Equal(StatusGuia.Vencida, guia.Status);
    }
}
