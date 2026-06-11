using Folha360.Esocial.Domain;
using Folha360.Esocial.Domain.Entities;

namespace Folha360.Tests.Esocial.Domain;

public class EventoEsocialTests
{
    [Fact]
    public void CriarEvento_DeveInicializarComStatusPendente()
    {
        // Arrange & Act
        var evento = new EventoEsocial(
            Guid.NewGuid(),
            TipoEventoEsocial.S1200,
            "<xml>test</xml>",
            "ID1234567890");

        // Assert
        Assert.Equal(StatusEvento.Pendente, evento.Status);
        Assert.NotEqual(Guid.Empty, evento.Id);
    }

    [Fact]
    public void Validar_DeveMudarStatusParaValidado()
    {
        // Arrange
        var evento = new EventoEsocial(
            Guid.NewGuid(),
            TipoEventoEsocial.S2200,
            "<xml>test</xml>",
            "ID1234567890");

        // Act
        evento.Validar();

        // Assert
        Assert.Equal(StatusEvento.Validado, evento.Status);
    }

    [Fact]
    public void Validar_EventoJaValidado_DeveLancarExcecao()
    {
        // Arrange
        var evento = new EventoEsocial(
            Guid.NewGuid(),
            TipoEventoEsocial.S2200,
            "<xml>test</xml>",
            "ID1234567890");
        evento.Validar();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evento.Validar());
    }

    [Fact]
    public void Assinar_DeveRegistrarCertificadoEHash()
    {
        // Arrange
        var evento = new EventoEsocial(
            Guid.NewGuid(),
            TipoEventoEsocial.S1200,
            "<xml>test</xml>",
            "ID1234567890");
        evento.Validar();
        var certificadoId = Guid.NewGuid();
        var hash = "ABC123";

        // Act
        evento.Assinar(certificadoId, hash);

        // Assert
        Assert.Equal(StatusEvento.Assinado, evento.Status);
        Assert.Equal(certificadoId, evento.CertificadoId);
        Assert.Equal(hash, evento.HashAssinatura);
    }
}

public class LoteEsocialTests
{
    [Fact]
    public void CriarLote_DeveInicializarComStatusPendente()
    {
        // Arrange & Act
        var lote = new LoteEsocial(Guid.NewGuid(), TipoAmbiente.Producao);

        // Assert
        Assert.Equal(StatusLote.Pendente, lote.Status);
        Assert.Equal(0, lote.QuantidadeEventos);
    }

    [Fact]
    public void Enviar_DeveRegistrarProtocoloEData()
    {
        // Arrange
        var lote = new LoteEsocial(Guid.NewGuid(), TipoAmbiente.Homologacao);
        lote.IniciarAssinatura(10);
        lote.ConcluirAssinatura();

        // Act
        lote.Enviar("PROTOCOLO-123");

        // Assert
        Assert.Equal(StatusLote.Enviado, lote.Status);
        Assert.Equal("PROTOCOLO-123", lote.ProtocoloEnvio);
        Assert.NotNull(lote.DataEnvio);
    }
}

public class CertificadoDigitalTests
{
    [Fact]
    public void EstaExpirado_DataExpiracaoPassada_DeveRetornarTrue()
    {
        // Arrange
        var certificado = new CertificadoDigital(
            Guid.NewGuid(),
            TipoCertificado.A1,
            "AC Teste",
            "12345678901234",
            DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        Assert.True(certificado.EstaExpirado);
    }

    [Fact]
    public void DiasRestantes_DeveCalcularCorretamente()
    {
        // Arrange
        var dias = 30;
        var certificado = new CertificadoDigital(
            Guid.NewGuid(),
            TipoCertificado.A1,
            "AC Teste",
            "12345678901234",
            DateTime.UtcNow.AddDays(dias));

        // Act & Assert
        Assert.Equal(dias, certificado.DiasRestantes);
    }

    [Fact]
    public void EstaExpirandoEmBreve_DentroDoLimite_DeveRetornarTrue()
    {
        // Arrange
        var certificado = new CertificadoDigital(
            Guid.NewGuid(),
            TipoCertificado.A1,
            "AC Teste",
            "12345678901234",
            DateTime.UtcNow.AddDays(15));

        // Act & Assert
        Assert.True(certificado.EstaExpirandoEmBreve(30));
    }
}

public class FalhaEsocialTests
{
    [Fact]
    public void IncrementarTentativa_DeveAumentarContador()
    {
        // Arrange
        var falha = new FalhaEsocial(
            Guid.NewGuid(),
            TipoErroEsocial.Validacao,
            "Erro de validação XSD");

        // Act
        falha.IncrementarTentativa();

        // Assert
        Assert.Equal(2, falha.Tentativas);
    }

    [Fact]
    public void Resolver_DeveDefinirDataResolucao()
    {
        // Arrange
        var falha = new FalhaEsocial(
            Guid.NewGuid(),
            TipoErroEsocial.Governo,
            "Erro do governo");

        // Act
        falha.Resolver();

        // Assert
        Assert.NotNull(falha.ResolvidoEm);
    }
}
