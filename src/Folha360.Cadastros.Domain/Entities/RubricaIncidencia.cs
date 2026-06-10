using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaIncidencia : BaseEntity
{
    public Guid RubricaId { get; private set; }
    public string TipoIncidencia { get; private set; } = null!;
    public Rubrica Rubrica { get; private set; } = null!;

    private RubricaIncidencia()
    {
    }

    public RubricaIncidencia(Guid rubricaId, string tipoIncidencia)
    {
        Id = Guid.NewGuid();
        RubricaId = rubricaId;
        TipoIncidencia = tipoIncidencia;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
