using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Domain.Enums;

namespace Folha360.Tests.Relatorios.Domain;

[Trait("Category", "Unit")]
public class RelatorioAgendamentoTests
{
    [Fact]
    public void Criar_DeveCriarAgendamentoAtivo()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var tipoRelatorio = TipoRelatorio.FolhaAnalitica;
        var formato = FormatoExportacao.Pdf;
        var recorrencia = "0 0 5 * * ?"; // 5º dia útil (simplificado)
        var destinatarios = "contador@empresa.com.br";

        // Act
        var agendamento = RelatorioAgendamento.Criar(empresaId, tipoRelatorio, formato, recorrencia, destinatarios);

        // Assert
        Assert.NotEqual(Guid.Empty, agendamento.Id);
        Assert.Equal(empresaId, agendamento.EmpresaId);
        Assert.Equal(tipoRelatorio, agendamento.TipoRelatorio);
        Assert.Equal(formato, agendamento.Formato);
        Assert.Equal(recorrencia, agendamento.Recorrencia);
        Assert.Equal(destinatarios, agendamento.Destinatarios);
        Assert.True(agendamento.Ativo);
        Assert.Null(agendamento.DeletedAt);
        Assert.Null(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Atualizar_DeveAlterarRecorrencia()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, FormatoExportacao.Pdf,
            "0 0 5 * * ?", "test@test.com");

        // Act
        agendamento.Atualizar(recorrencia: "0 0 8 * * ?", formato: null, destinatarios: null, ativo: null);

        // Assert
        Assert.Equal("0 0 8 * * ?", agendamento.Recorrencia);
        Assert.NotNull(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Atualizar_DeveAlterarFormato()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, FormatoExportacao.Pdf,
            "0 0 5 * * ?", "test@test.com");

        // Act
        agendamento.Atualizar(recorrencia: null, formato: FormatoExportacao.Csv, destinatarios: null, ativo: null);

        // Assert
        Assert.Equal(FormatoExportacao.Csv, agendamento.Formato);
        Assert.NotNull(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Atualizar_DeveAlterarDestinatarios()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, FormatoExportacao.Pdf,
            "0 0 5 * * ?", "old@test.com");

        // Act
        agendamento.Atualizar(recorrencia: null, formato: null, destinatarios: "new@test.com", ativo: null);

        // Assert
        Assert.Equal("new@test.com", agendamento.Destinatarios);
        Assert.NotNull(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Atualizar_DeveAlterarAtivo()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, FormatoExportacao.Pdf,
            "0 0 5 * * ?", "test@test.com");

        // Act
        agendamento.Atualizar(recorrencia: null, formato: null, destinatarios: null, ativo: false);

        // Assert
        Assert.False(agendamento.Ativo);
        Assert.NotNull(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Atualizar_NaoDeveAlterarPropriedadesComValorNulo()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.FolhaAnalitica, FormatoExportacao.Pdf,
            "0 0 5 * * ?", "test@test.com");
        var recorrenciaOriginal = agendamento.Recorrencia;
        var formatoOriginal = agendamento.Formato;
        var destinatariosOriginal = agendamento.Destinatarios;

        // Act
        agendamento.Atualizar(recorrencia: null, formato: null, destinatarios: null, ativo: null);

        // Assert
        Assert.Equal(recorrenciaOriginal, agendamento.Recorrencia);
        Assert.Equal(formatoOriginal, agendamento.Formato);
        Assert.Equal(destinatariosOriginal, agendamento.Destinatarios);
        Assert.True(agendamento.Ativo); // não foi alterado
        Assert.NotNull(agendamento.AtualizadoEm); // atualizadoEm sempre é setado
    }

    [Fact]
    public void Cancelar_DeveMarcarComoInativoEPreencherDeletedAt()
    {
        // Arrange
        var agendamento = RelatorioAgendamento.Criar(
            Guid.NewGuid(), TipoRelatorio.ResumoMensal, FormatoExportacao.Json,
            "0 0 10 * * ?", "admin@test.com");

        // Act
        agendamento.Cancelar();

        // Assert
        Assert.False(agendamento.Ativo);
        Assert.NotNull(agendamento.DeletedAt);
        Assert.NotNull(agendamento.AtualizadoEm);
    }

    [Fact]
    public void Criar_DiferentesChamadas_DevemGerarIdsDiferentes()
    {
        // Act
        var a1 = RelatorioAgendamento.Criar(Guid.NewGuid(), TipoRelatorio.Holerite, FormatoExportacao.Pdf, "0 0 5 * * ?", "a@test.com");
        var a2 = RelatorioAgendamento.Criar(Guid.NewGuid(), TipoRelatorio.Holerite, FormatoExportacao.Pdf, "0 0 5 * * ?", "a@test.com");

        // Assert
        Assert.NotEqual(a1.Id, a2.Id);
    }
}
