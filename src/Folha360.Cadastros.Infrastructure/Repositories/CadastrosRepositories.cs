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

// ============================
// Subsistema de Rubricas (ADR-006)
// ============================

// --- GrupoRubrica Repository ---
public class GrupoRubricaRepository : IGrupoRubricaRepository
{
    private readonly CadastrosDbContext _db;
    public GrupoRubricaRepository(CadastrosDbContext db) => _db = db;

    public async Task<GrupoRubrica?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.GruposRubrica.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<GrupoRubrica?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default)
        => await _db.GruposRubrica.FirstOrDefaultAsync(g => g.EmpresaId == empresaId && g.Codigo == codigo, ct);

    public async Task<IEnumerable<GrupoRubrica>> GetAllByEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.GruposRubrica.Where(g => g.EmpresaId == empresaId).OrderBy(g => g.OrdemExibicao).ToListAsync(ct);

    public async Task<(IEnumerable<GrupoRubrica> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? natureza = null,
        CancellationToken ct = default)
    {
        var query = _db.GruposRubrica.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(g => g.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(natureza))
            query = query.Where(g => g.Natureza == natureza);

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(g => g.OrdemExibicao).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task<bool> HasRubricasVinculadasAsync(Guid id, CancellationToken ct = default)
        => await _db.Rubricas.AnyAsync(r => r.GrupoRubricaId == id, ct);

    public async Task AddAsync(GrupoRubrica grupo, CancellationToken ct = default)
    {
        _db.GruposRubrica.Add(grupo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(GrupoRubrica grupo, CancellationToken ct = default)
    {
        _db.GruposRubrica.Update(grupo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var grupo = await _db.GruposRubrica.FindAsync([id], ct);
        if (grupo is not null)
        {
            _db.GruposRubrica.Remove(grupo);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// --- RubricaComposicao Repository ---
public class RubricaComposicaoRepository : IRubricaComposicaoRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaComposicaoRepository(CadastrosDbContext db) => _db = db;

    public async Task<IEnumerable<RubricaComposicao>> GetByPrincipalAsync(Guid rubricaPrincipalId, CancellationToken ct = default)
        => await _db.RubricasComposicao
            .Where(c => c.RubricaPrincipalId == rubricaPrincipalId)
            .OrderBy(c => c.Ordem)
            .ToListAsync(ct);

    public async Task<RubricaComposicao?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RubricasComposicao.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> ExistsCaminhoAsync(Guid origemId, Guid destinoId, CancellationToken ct = default)
    {
        // DFS para detectar se já existe caminho do componente para o principal
        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(origemId);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current == destinoId)
                return true;
            if (!visited.Add(current))
                continue;

            var filhos = await _db.RubricasComposicao
                .Where(c => c.RubricaPrincipalId == current)
                .Select(c => c.RubricaComponenteId)
                .ToListAsync(ct);

            foreach (var filho in filhos)
                stack.Push(filho);
        }

        return false;
    }

    public async Task AddAsync(RubricaComposicao composicao, CancellationToken ct = default)
    {
        _db.RubricasComposicao.Add(composicao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var comp = await _db.RubricasComposicao.FindAsync([id], ct);
        if (comp is not null)
        {
            _db.RubricasComposicao.Remove(comp);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// --- RubricaFormula Repository ---
public class RubricaFormulaRepository : IRubricaFormulaRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaFormulaRepository(CadastrosDbContext db) => _db = db;

    public async Task<RubricaFormula?> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default)
        => await _db.RubricasFormula.FirstOrDefaultAsync(f => f.RubricaId == rubricaId, ct);

    public async Task AddAsync(RubricaFormula formula, CancellationToken ct = default)
    {
        _db.RubricasFormula.Add(formula);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RubricaFormula formula, CancellationToken ct = default)
    {
        _db.RubricasFormula.Update(formula);
        await _db.SaveChangesAsync(ct);
    }
}

// --- RubricaTabelaProgressiva Repository ---
public class RubricaTabelaProgressivaRepository : IRubricaTabelaProgressivaRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaTabelaProgressivaRepository(CadastrosDbContext db) => _db = db;

    public async Task<IEnumerable<RubricaTabelaProgressiva>> GetByAnoVigenciaAsync(Guid rubricaId, int ano, CancellationToken ct = default)
        => await _db.RubricasTabelaProgressiva
            .Where(t => t.RubricaId == rubricaId && t.AnoVigencia == ano)
            .OrderBy(t => t.Ordem)
            .ToListAsync(ct);

    public async Task<IEnumerable<RubricaTabelaProgressiva>> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default)
        => await _db.RubricasTabelaProgressiva
            .Where(t => t.RubricaId == rubricaId)
            .OrderBy(t => t.AnoVigencia).ThenBy(t => t.Ordem)
            .ToListAsync(ct);

    public async Task<RubricaTabelaProgressiva?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RubricasTabelaProgressiva.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<bool> HasSobreposicaoAsync(Guid rubricaId, int anoVigencia, decimal faixaDe, decimal? faixaAte, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = _db.RubricasTabelaProgressiva
            .Where(t => t.RubricaId == rubricaId && t.AnoVigencia == anoVigencia);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        // Verifica se alguma faixa existente se sobrepõe com [faixaDe, faixaAte]
        var faixas = await query.ToListAsync(ct);
        foreach (var faixa in faixas)
        {
            var existingEnd = faixa.FaixaAte ?? decimal.MaxValue;
            var newEnd = faixaAte ?? decimal.MaxValue;

            // Sobreposição: [faixaDe, newEnd] ∩ [faixa.FaixaDe, existingEnd] ≠ ∅
            if (faixaDe < existingEnd && newEnd > faixa.FaixaDe)
                return true;
        }

        return false;
    }

    public async Task AddAsync(RubricaTabelaProgressiva faixa, CancellationToken ct = default)
    {
        _db.RubricasTabelaProgressiva.Add(faixa);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RubricaTabelaProgressiva faixa, CancellationToken ct = default)
    {
        _db.RubricasTabelaProgressiva.Update(faixa);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var faixa = await _db.RubricasTabelaProgressiva.FindAsync([id], ct);
        if (faixa is not null)
        {
            _db.RubricasTabelaProgressiva.Remove(faixa);
            await _db.SaveChangesAsync(ct);
        }
    }
}

// --- RubricaHistorico Repository ---
public class RubricaHistoricoRepository : IRubricaHistoricoRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaHistoricoRepository(CadastrosDbContext db) => _db = db;

    public async Task<(IEnumerable<RubricaHistorico> Items, int TotalCount)> GetByRubricaPagedAsync(
        Guid rubricaId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.RubricasHistorico.Where(h => h.RubricaId == rubricaId);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(h => h.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(RubricaHistorico historico, CancellationToken ct = default)
    {
        _db.RubricasHistorico.Add(historico);
        await _db.SaveChangesAsync(ct);
    }
}

// --- RubricaIncidencia Repository ---
public class RubricaIncidenciaRepository : IRubricaIncidenciaRepository
{
    private readonly CadastrosDbContext _db;
    public RubricaIncidenciaRepository(CadastrosDbContext db) => _db = db;

    public async Task<IEnumerable<RubricaIncidencia>> GetByRubricaAsync(Guid rubricaId, CancellationToken ct = default)
        => await _db.RubricasIncidencia.Where(i => i.RubricaId == rubricaId).ToListAsync(ct);

    public async Task<RubricaIncidencia?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RubricasIncidencia.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<bool> ExistsAsync(Guid rubricaId, string tipoIncidencia, CancellationToken ct = default)
        => await _db.RubricasIncidencia.AnyAsync(i => i.RubricaId == rubricaId && i.TipoIncidencia == tipoIncidencia, ct);

    public async Task AddAsync(RubricaIncidencia incidencia, CancellationToken ct = default)
    {
        _db.RubricasIncidencia.Add(incidencia);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var inc = await _db.RubricasIncidencia.FindAsync([id], ct);
        if (inc is not null)
        {
            _db.RubricasIncidencia.Remove(inc);
            await _db.SaveChangesAsync(ct);
        }
    }
}
