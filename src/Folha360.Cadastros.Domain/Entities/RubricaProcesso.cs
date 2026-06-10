using Folha360.Domain;

namespace Folha360.Cadastros.Domain.Entities;

public class RubricaProcesso : BaseEntity
{
    public Guid RubricaId { get; private set; }
    public Guid ProcessoAdministrativoId { get; private set; }
    public Rubrica Rubrica { get; private set; } = null!;
    public ProcessoAdministrativo ProcessoAdministrativo { get; private set; } = null!;

    private RubricaProcesso()
    {
    }

    public RubricaProcesso(Guid rubricaId, Guid processoAdministrativoId)
    {
        Id = Guid.NewGuid();
        RubricaId = rubricaId;
        ProcessoAdministrativoId = processoAdministrativoId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
