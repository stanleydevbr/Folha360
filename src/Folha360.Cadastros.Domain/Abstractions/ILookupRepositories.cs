using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

public interface ICboRepository
{
    Task<CboOcupacao?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CboOcupacao>> ListarAsync(string? filtro = null, bool apenasAtivos = true, CancellationToken ct = default);
}

public interface INaturezaJuridicaRepository
{
    Task<NaturezaJuridica?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<NaturezaJuridica>> ListarAsync(bool apenasAtivos = true, CancellationToken ct = default);
}

public interface IMunicipioIBGERepository
{
    Task<MunicipioIBGE?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MunicipioIBGE?> ObterPorCodigoAsync(string codigoIbge, CancellationToken ct = default);
    Task<(IEnumerable<MunicipioIBGE> Items, int TotalCount)> ListarAsync(
        string? uf = null, string? nome = null, int page = 1, int pageSize = 50,
        bool apenasAtivos = true, CancellationToken ct = default);
}

public interface IBancoFebrabanRepository
{
    Task<BancoFebraban?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BancoFebraban>> ListarAsync(string? filtro = null, bool apenasAtivos = true, CancellationToken ct = default);
}
