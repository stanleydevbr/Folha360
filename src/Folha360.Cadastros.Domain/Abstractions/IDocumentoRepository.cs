using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IDocumentoRepository
{
    Task<Documento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Documento>> GetByFuncionarioIdAsync(Guid funcionarioId, CancellationToken ct = default);
    Task<Documento?> GetByFuncionarioIdAndTipoAsync(Guid funcionarioId, string tipo, CancellationToken ct = default);
    Task<(IEnumerable<Documento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? funcionarioId = null, string? tipo = null,
        CancellationToken ct = default);
    Task AddAsync(Documento documento, CancellationToken ct = default);
    Task UpdateAsync(Documento documento, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
