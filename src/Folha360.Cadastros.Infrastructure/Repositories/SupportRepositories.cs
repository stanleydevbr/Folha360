using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Repositories;

// ============================
// Cadastros de Apoio (Sindicatos, Convênios, Horários, Dependentes, Documentos)
// ============================
public class SindicatoRepository : ISindicatoRepository
{
    private readonly CadastrosDbContext _db;
    public SindicatoRepository(CadastrosDbContext db) => _db = db;

    public async Task<Sindicato?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Sindicatos.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<Sindicato?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default)
        => await _db.Sindicatos.FirstOrDefaultAsync(s => s.EmpresaId == empresaId && s.Codigo == codigo, ct);

    public async Task<IEnumerable<Sindicato>> GetAllAsync(CancellationToken ct = default)
        => await _db.Sindicatos.ToListAsync(ct);

    public async Task<(IEnumerable<Sindicato> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? nome = null,
        CancellationToken ct = default)
    {
        var query = _db.Sindicatos.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(s => s.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(s => s.Nome.Contains(nome));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Sindicato entity, CancellationToken ct = default)
    {
        _db.Sindicatos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Sindicato entity, CancellationToken ct = default)
    {
        _db.Sindicatos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Sindicatos.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.Sindicatos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class ConvenioRepository : IConvenioRepository
{
    private readonly CadastrosDbContext _db;
    public ConvenioRepository(CadastrosDbContext db) => _db = db;

    public async Task<Convenio?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Convenios.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<Convenio>> GetAllAsync(CancellationToken ct = default)
        => await _db.Convenios.ToListAsync(ct);

    public async Task<(IEnumerable<Convenio> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? nome = null,
        CancellationToken ct = default)
    {
        var query = _db.Convenios.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(c => c.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(c => c.Nome.Contains(nome));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Convenio entity, CancellationToken ct = default)
    {
        _db.Convenios.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Convenio entity, CancellationToken ct = default)
    {
        _db.Convenios.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Convenios.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.Convenios.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class HorarioTrabalhoRepository : IHorarioTrabalhoRepository
{
    private readonly CadastrosDbContext _db;
    public HorarioTrabalhoRepository(CadastrosDbContext db) => _db = db;

    public async Task<HorarioTrabalho?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.HorariosTrabalho.FirstOrDefaultAsync(h => h.Id == id, ct);

    public async Task<HorarioTrabalho?> GetByCodigoAsync(Guid empresaId, string codigo, CancellationToken ct = default)
        => await _db.HorariosTrabalho.FirstOrDefaultAsync(h => h.EmpresaId == empresaId && h.Codigo == codigo, ct);

    public async Task<IEnumerable<HorarioTrabalho>> GetAllAsync(CancellationToken ct = default)
        => await _db.HorariosTrabalho.ToListAsync(ct);

    public async Task<(IEnumerable<HorarioTrabalho> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? empresaId = null, string? descricao = null,
        CancellationToken ct = default)
    {
        var query = _db.HorariosTrabalho.AsQueryable();
        if (empresaId.HasValue)
            query = query.Where(h => h.EmpresaId == empresaId.Value);
        if (!string.IsNullOrWhiteSpace(descricao))
            query = query.Where(h => h.Descricao.Contains(descricao));

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(HorarioTrabalho entity, CancellationToken ct = default)
    {
        _db.HorariosTrabalho.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(HorarioTrabalho entity, CancellationToken ct = default)
    {
        _db.HorariosTrabalho.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.HorariosTrabalho.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.HorariosTrabalho.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class DependenteRepository : IDependenteRepository
{
    private readonly CadastrosDbContext _db;
    public DependenteRepository(CadastrosDbContext db) => _db = db;

    public async Task<Dependente?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Dependentes.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<Dependente>> GetByFuncionarioIdAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.Dependentes.Where(d => d.FuncionarioId == funcionarioId).ToListAsync(ct);

    public async Task<(IEnumerable<Dependente> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? funcionarioId = null, string? tipo = null,
        CancellationToken ct = default)
    {
        var query = _db.Dependentes.AsQueryable();
        if (funcionarioId.HasValue)
            query = query.Where(d => d.FuncionarioId == funcionarioId.Value);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(d => d.Tipo == tipo);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Dependente entity, CancellationToken ct = default)
    {
        _db.Dependentes.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Dependente entity, CancellationToken ct = default)
    {
        _db.Dependentes.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Dependentes.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.Dependentes.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class DocumentoRepository : IDocumentoRepository
{
    private readonly CadastrosDbContext _db;
    public DocumentoRepository(CadastrosDbContext db) => _db = db;

    public async Task<Documento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Documentos.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<Documento>> GetByFuncionarioIdAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.Documentos.Where(d => d.FuncionarioId == funcionarioId).ToListAsync(ct);

    public async Task<Documento?> GetByFuncionarioIdAndTipoAsync(Guid funcionarioId, string tipo, CancellationToken ct = default)
        => await _db.Documentos.FirstOrDefaultAsync(d => d.FuncionarioId == funcionarioId && d.Tipo == tipo, ct);

    public async Task<(IEnumerable<Documento> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? orderBy = null,
        Guid? funcionarioId = null, string? tipo = null,
        CancellationToken ct = default)
    {
        var query = _db.Documentos.AsQueryable();
        if (funcionarioId.HasValue)
            query = query.Where(d => d.FuncionarioId == funcionarioId.Value);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(d => d.Tipo == tipo);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Documento entity, CancellationToken ct = default)
    {
        _db.Documentos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Documento entity, CancellationToken ct = default)
    {
        _db.Documentos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Documentos.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.Documentos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
