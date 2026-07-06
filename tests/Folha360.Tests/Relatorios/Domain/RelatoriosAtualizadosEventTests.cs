using Folha360.Relatorios.Domain.Events;

namespace Folha360.Tests.Relatorios.Domain;

[Trait("Category", "Unit")]
public class RelatoriosAtualizadosEventTests
{
    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var periodo = "2026-05";
        var tipos = new List<string> { "folha_analitica", "folha_sintetica", "resumo_mensal" };
        var correlationId = Guid.NewGuid();
        var causationId = Guid.NewGuid();

        // Act
        var evento = new RelatoriosAtualizadosEvent
        {
            EmpresaId = empresaId,
            Periodo = periodo,
            TiposRelatorio = tipos,
            CorrelationId = correlationId,
            CausationId = causationId,
        };

        // Assert
        Assert.Equal(empresaId, evento.EmpresaId);
        Assert.Equal(periodo, evento.Periodo);
        Assert.Equal(tipos, evento.TiposRelatorio);
        Assert.Equal(correlationId, evento.CorrelationId);
        Assert.Equal(causationId, evento.CausationId);
    }

    [Fact]
    public void TiposRelatorio_DeveSerListaVaziaPorPadrao()
    {
        // Act
        var evento = new RelatoriosAtualizadosEvent();

        // Assert
        Assert.NotNull(evento.TiposRelatorio);
        Assert.Empty(evento.TiposRelatorio);
    }

    [Fact]
    public void Periodo_DeveSerStringVaziaPorPadrao()
    {
        // Act
        var evento = new RelatoriosAtualizadosEvent();

        // Assert
        Assert.Equal(string.Empty, evento.Periodo);
    }

    [Fact]
    public void CorrelationId_DeveSerPropagadoCorretamente()
    {
        // Este teste valida que o evento carrega o correlation_id
        // para tracing distribuído entre F04 → F06 → F08
        var correlationId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var evento = new RelatoriosAtualizadosEvent
        {
            CorrelationId = correlationId,
            CausationId = Guid.NewGuid(),
        };

        Assert.Equal(correlationId, evento.CorrelationId);
    }

    [Fact]
    public void CausationId_DeveReferenciarEventoOriginal()
    {
        // Este teste valida que o causation_id referencia o evento
        // que originou esta publicação (ex: FolhaFechadaEvent.MessageId)
        var causationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var evento = new RelatoriosAtualizadosEvent
        {
            CorrelationId = Guid.NewGuid(),
            CausationId = causationId,
        };

        Assert.Equal(causationId, evento.CausationId);
    }
}
