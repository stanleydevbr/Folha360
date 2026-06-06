namespace Folha360.Cadastros.Domain.Events;

public record RubricaAlteradaEvent(
    Guid RubricaId,
    Guid EmpresaId,
    string Codigo,
    string Natureza,
    RubricaIncidencias Incidencias,
    DateTime OcorridoEm)
{
    public RubricaAlteradaEvent(
        Guid rubricaId,
        Guid empresaId,
        string codigo,
        string natureza,
        RubricaIncidencias incidencias)
        : this(rubricaId, empresaId, codigo, natureza, incidencias, DateTime.UtcNow)
    {
    }
}

public record RubricaIncidencias(
    bool IncideInss,
    bool IncideIrrf,
    bool IncideFgts,
    bool IncideContribuicaoSindical,
    bool IncideDecimoTerceiro,
    bool IncideFerias,
    bool IncideAvisoPrevio);
