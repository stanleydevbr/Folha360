using Folha360.Fiscais.Domain.Entities;

namespace Folha360.Fiscais.Domain.Abstractions;

public interface IGuiaRecolhimentoRepository
{
    Task<GuiaRecolhimento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GuiaRecolhimento>> GetByEmpresaPeriodoAsync(Guid empresaId, DateOnly periodo, CancellationToken ct = default);
    Task<IReadOnlyList<GuiaRecolhimento>> GetPendentesAsync(Guid empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<GuiaRecolhimento>> GetVencidasAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(GuiaRecolhimento guia, CancellationToken ct = default);
    Task UpdateAsync(GuiaRecolhimento guia, CancellationToken ct = default);
}
