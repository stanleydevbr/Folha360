using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IConvenioRepository
{
    Task<Convenio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Convenio>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Convenio> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? tipo = null,
        CancellationToken ct = default);
    Task AddAsync(Convenio convenio, CancellationToken ct = default);
    Task UpdateAsync(Convenio convenio, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
