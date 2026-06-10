using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaHistorico : BaseEntity
{
    public Guid RubricaId { get; private set; }
    public string? DadosAnteriores { get; private set; }
    public string DadosNovos { get; private set; } = null!;
    public string? Motivo { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Rubrica Rubrica { get; private set; } = null!;

    private RubricaHistorico()
    {
    }

    public RubricaHistorico(Guid rubricaId, string? dadosAnteriores, string dadosNovos, Guid usuarioId, string? motivo = null)
    {
        Id = Guid.NewGuid();
        RubricaId = rubricaId;
        DadosAnteriores = dadosAnteriores;
        DadosNovos = dadosNovos;
        UsuarioId = usuarioId;
        Motivo = motivo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
