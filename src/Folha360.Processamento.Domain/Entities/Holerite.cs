using Folha360.Domain;

namespace Folha360.Processamento.Domain.Entities;

public class Holerite : BaseEntity
{
    public Guid ProcessamentoId { get; private set; }
    public Guid FuncionarioId { get; private set; }
    public string MinioKey { get; private set; } = null!;
    public DateTime DataGeracao { get; private set; }

    private Holerite()
    {
    }

    public Holerite(Guid processamentoId, Guid funcionarioId, string minioKey)
    {
        Id = Guid.NewGuid();
        ProcessamentoId = processamentoId;
        FuncionarioId = funcionarioId;
        MinioKey = minioKey;
        DataGeracao = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
