using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Validators;
using Folha360.Eventos.Domain;

namespace Folha360.Tests.Eventos.Validators;

public class CriarFeriasCommandValidatorTests
{
    private readonly CriarFeriasCommandValidator _validator = new();

    [Fact]
    public void DeveSerValido_ComAntecedenciaMinima15Dias()
    {
        var cmd = new CriarFeriasCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
            DiasGozo = 30,
            PeriodoAquisitivoInicio = new DateOnly(2025, 6, 1),
            PeriodoAquisitivoFim = new DateOnly(2026, 5, 31),
            TipoFerias = TipoFerias.Normais,
        };

        var result = _validator.Validate(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveSerInvalido_ComAntecedenciaMenorQue15Dias()
    {
        var cmd = new CriarFeriasCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            DiasGozo = 30,
            PeriodoAquisitivoInicio = new DateOnly(2025, 6, 1),
            PeriodoAquisitivoFim = new DateOnly(2026, 5, 31),
            TipoFerias = TipoFerias.Normais,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "PRAZO_FERIAS_EXCEDIDO");
    }

    [Fact]
    public void DeveSerInvalido_ComPeriodoAquisitivoInvertido()
    {
        var cmd = new CriarFeriasCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(20)),
            DiasGozo = 30,
            PeriodoAquisitivoInicio = new DateOnly(2026, 5, 31),
            PeriodoAquisitivoFim = new DateOnly(2025, 6, 1),
            TipoFerias = TipoFerias.Normais,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
    }
}
