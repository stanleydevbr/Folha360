using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Repositories;

// ============================
// Empresa Repository
// ============================
public class EmpresaRepository : IEmpresaRepository
{
    private readonly CadastrosDbContext _db;
    public EmpresaRepository(CadastrosDbContext db) => _db = db;

    public async Task<Empresa?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Empresas.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
        => await _db.Empresas.IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Cnpj == cnpj, ct);

    public async Task<IEnumerable<Empresa>> GetAllAsync(CancellationToken ct = default)
        => await _db.Empresas.ToListAsync(ct);

    public async Task<(IEnumerable<Empresa> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        string? cnpj = null, string? razaoSocial = null, string? regimeTributario = null,
        CancellationToken ct = default)
    {
        var query = _db.Empresas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(cnpj))
            query = query.Where(e => e.Cnpj.Contains(cnpj));
        if (!string.IsNullOrWhiteSpace(razaoSocial))
            query = query.Where(e => e.RazaoSocial.Contains(razaoSocial));
        if (!string.IsNullOrWhiteSpace(regimeTributario))
            query = query.Where(e => e.RegimeTributario == regimeTributario);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Empresa empresa, CancellationToken ct = default)
    {
        _db.Empresas.Add(empresa);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Empresa empresa, CancellationToken ct = default)
    {
        _db.Empresas.Update(empresa);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var empresa = await _db.Empresas.FindAsync([id], ct);
        if (empresa is not null)
        {
            _db.Empresas.Remove(empresa);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// ============================
// Funcionario Repository
// ============================
public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly CadastrosDbContext _db;
    public FuncionarioRepository(CadastrosDbContext db) => _db = db;

    public async Task<Funcionario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Funcionarios.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<Funcionario?> GetByCpfHashAsync(string cpfHash, CancellationToken ct = default)
        => await _db.Funcionarios.IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.CpfHash == cpfHash, ct);

    public async Task<IEnumerable<Funcionario>> GetAllAsync(CancellationToken ct = default)
        => await _db.Funcionarios.ToListAsync(ct);

    public async Task<(IEnumerable<Funcionario> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? status = null, Guid? cargoId = null,
        Guid? lotacaoId = null, string? nome = null, CancellationToken ct = default)
    {
        var query = _db.Funcionarios.AsQueryable();

        if (empresaId.HasValue)
            query = query.Where(f => f.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(f => f.Status == status);
        if (cargoId.HasValue)
            query = query.Where(f => f.CargoId == cargoId.Value);
        if (lotacaoId.HasValue)
            query = query.Where(f => f.LotacaoId == lotacaoId.Value);
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(f => f.Nome.Contains(nome));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Funcionario funcionario, CancellationToken ct = default)
    {
        _db.Funcionarios.Add(funcionario);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Funcionario funcionario, CancellationToken ct = default)
    {
        _db.Funcionarios.Update(funcionario);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var f = await _db.Funcionarios.FindAsync([id], ct);
        if (f is not null)
        {
            _db.Funcionarios.Remove(f);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// ============================
// Cargo Repository
// ============================
public class CargoRepository : ICargoRepository
{
    private readonly CadastrosDbContext _db;
    public CargoRepository(CadastrosDbContext db) => _db = db;

    public async Task<Cargo?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Cargos.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Cargo>> GetAllAsync(CancellationToken ct = default)
        => await _db.Cargos.ToListAsync(ct);

    public async Task<(IEnumerable<Cargo> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? nome = null, CancellationToken ct = default)
    {
        var query = _db.Cargos.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(c => c.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(c => c.Nome.Contains(nome));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Cargo cargo, CancellationToken ct = default)
    {
        _db.Cargos.Add(cargo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Cargo cargo, CancellationToken ct = default)
    {
        _db.Cargos.Update(cargo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _db.Cargos.FindAsync([id], ct);
        if (c is not null)
        {
            _db.Cargos.Remove(c);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> HasFuncionariosVinculadosAsync(Guid id, CancellationToken ct = default)
        => await _db.Funcionarios.AnyAsync(f => f.CargoId == id, ct);
}

// ============================
// Rubrica Repository
// ============================
public class RubricaRepository : IRubricaRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaRepository(CadastrosDbContext db) => _db = db;

    public async Task<Rubrica?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Rubricas.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<Rubrica?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default)
        => await _db.Rubricas.FirstOrDefaultAsync(r => r.EmpresaId == empresaId && r.Codigo == codigo, ct);

    public async Task<IEnumerable<Rubrica>> GetAllAsync(CancellationToken ct = default)
        => await _db.Rubricas.ToListAsync(ct);

    public async Task<IEnumerable<Rubrica>> GetAllByEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.Rubricas.Where(r => r.EmpresaId == empresaId).ToListAsync(ct);

    public async Task<(IEnumerable<Rubrica> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? natureza = null, string? tipoEsocial = null,
        CancellationToken ct = default)
    {
        var query = _db.Rubricas.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(r => r.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(natureza))
            query = query.Where(r => r.Natureza == natureza);
        if (!string.IsNullOrWhiteSpace(tipoEsocial))
            query = query.Where(r => r.TipoEsocial == tipoEsocial);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Rubrica rubrica, CancellationToken ct = default)
    {
        _db.Rubricas.Add(rubrica);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Rubrica rubrica, CancellationToken ct = default)
    {
        _db.Rubricas.Update(rubrica);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var r = await _db.Rubricas.FindAsync([id], ct);
        if (r is not null)
        {
            _db.Rubricas.Remove(r);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// ============================
// Lotacao Repository
// ============================
public class LotacaoRepository : ILotacaoRepository
{
    private readonly CadastrosDbContext _db;
    public LotacaoRepository(CadastrosDbContext db) => _db = db;

    public async Task<Lotacao?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Lotacoes.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Lotacao?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default)
        => await _db.Lotacoes.FirstOrDefaultAsync(l => l.EmpresaId == empresaId && l.Codigo == codigo, ct);

    public async Task<IEnumerable<Lotacao>> GetAllAsync(CancellationToken ct = default)
        => await _db.Lotacoes.ToListAsync(ct);

    public async Task<(IEnumerable<Lotacao> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? codigo = null, CancellationToken ct = default)
    {
        var query = _db.Lotacoes.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(l => l.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(codigo))
            query = query.Where(l => l.Codigo.Contains(codigo));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Lotacao lotacao, CancellationToken ct = default)
    {
        _db.Lotacoes.Add(lotacao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Lotacao lotacao, CancellationToken ct = default)
    {
        _db.Lotacoes.Update(lotacao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var l = await _db.Lotacoes.FindAsync([id], ct);
        if (l is not null)
        {
            _db.Lotacoes.Remove(l);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> HasFuncionariosVinculadosAsync(Guid id, CancellationToken ct = default)
        => await _db.Funcionarios.AnyAsync(f => f.LotacaoId == id, ct);
}
