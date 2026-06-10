using Folha360.Eventos.Application.Commands;
using Folha360.Eventos.Application.Validators;
using Folha360.Eventos.Domain;

namespace Folha360.Tests.Eventos.Validators;

[Trait("Category", "Unit")]
public class CriarDesligamentoCommandValidatorTests
{
    private readonly CriarDesligamentoCommandValidator _validator = new();

    [Fact]
    public void DeveSerValido_ComDataDentroDoPrazo10Dias()
    {
        var cmd = new CriarDesligamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataDesligamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            MotivoDesligamento = MotivoDesligamento.SemJustaCausa,
        };

        var result = _validator.Validate(cmd);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DeveSerInvalido_ComPrazoDesligamentoExcedido()
    {
        var cmd = new CriarDesligamentoCommand
        {
            FuncionarioId = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            DataDesligamento = DateOnly.FromDateTime(DateTime.Today.AddDays(-11)),
            MotivoDesligamento = MotivoDesligamento.SemJustaCausa,
        };

        var result = _validator.Validate(cmd);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "PRAZO_DESLIGAMENTO_EXCEDIDO");
    }
}
