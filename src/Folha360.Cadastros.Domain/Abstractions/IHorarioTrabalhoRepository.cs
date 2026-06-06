using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IHorarioTrabalhoRepository
{
    Task<HorarioTrabalho?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<HorarioTrabalho?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default);
    Task<IEnumerable<HorarioTrabalho>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<HorarioTrabalho> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? tipo = null,
        CancellationToken ct = default);
    Task AddAsync(HorarioTrabalho horario, CancellationToken ct = default);
    Task UpdateAsync(HorarioTrabalho horario, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
