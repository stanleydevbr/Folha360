using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface IEmpresaRepository
{
    Task<Empresa?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IEnumerable<Empresa>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Empresa> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        string? cnpj = null, string? razaoSocial = null, string? regimeTributario = null,
        CancellationToken ct = default);
    Task AddAsync(Empresa empresa, CancellationToken ct = default);
    Task UpdateAsync(Empresa empresa, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
