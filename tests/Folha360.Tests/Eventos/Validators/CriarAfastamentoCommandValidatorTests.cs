using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Validators;
using Folha360.Eventos.Domain;

namespace Folha360.Tests.Eventos.Validators;

public class CriarAfastamentoCommandValidatorTests
{
    private readonly CriarAfastamentoCommandValidator _validator = new();

    [Fact]
    public void DeveSerValido_ComDataInicioPassadaEDataFimMaior()
    {
        var cmd = new CriarAfastamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            DataFimPrevista = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            TipoAfastamento = TipoAfastamento.Doenca,
        };

        var result = _validator.Validate(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveSerInvalido_ComDataInicioFutura()
    {
        var cmd = new CriarAfastamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            DataFimPrevista = DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
            TipoAfastamento = TipoAfastamento.Doenca,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "DATA_INICIO_FUTURA");
    }

    [Fact]
    public void DeveSerInvalido_ComDataFimMenorQueDataInicio()
    {
        var cmd = new CriarAfastamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            DataFimPrevista = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            TipoAfastamento = TipoAfastamento.Doenca,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "DATA_FIM_INVALIDA");
    }
}
