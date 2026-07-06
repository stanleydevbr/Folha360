using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Repositories;

// ============================
// Company Expansion (T12)
// ============================
public class ConfiguracaoBancariaEmpresaRepository : IConfiguracaoBancariaEmpresaRepository
{
    private readonly CadastrosDbContext _db;
    public ConfiguracaoBancariaEmpresaRepository(CadastrosDbContext db) => _db = db;

    public async Task<ConfiguracaoBancariaEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ConfiguracoesBancariasEmpresa.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<ConfiguracaoBancariaEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ConfiguracoesBancariasEmpresa.Where(c => c.EmpresaId == empresaId).ToListAsync(ct);

    public async Task AddAsync(ConfiguracaoBancariaEmpresa entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesBancariasEmpresa.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ConfiguracaoBancariaEmpresa entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesBancariasEmpresa.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ConfiguracoesBancariasEmpresa.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ConfiguracoesBancariasEmpresa.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class EnderecoEmpresaRepository : IEnderecoEmpresaRepository
{
    private readonly CadastrosDbContext _db;
    public EnderecoEmpresaRepository(CadastrosDbContext db) => _db = db;

    public async Task<EnderecoEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.EnderecosEmpresa.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<EnderecoEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.EnderecosEmpresa.Where(e => e.EmpresaId == empresaId).ToListAsync(ct);

    public async Task AddAsync(EnderecoEmpresa entity, CancellationToken ct = default)
    {
        _db.EnderecosEmpresa.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(EnderecoEmpresa entity, CancellationToken ct = default)
    {
        _db.EnderecosEmpresa.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.EnderecosEmpresa.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.EnderecosEmpresa.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class ContatoEmpresaRepository : IContatoEmpresaRepository
{
    private readonly CadastrosDbContext _db;
    public ContatoEmpresaRepository(CadastrosDbContext db) => _db = db;

    public async Task<ContatoEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ContatosEmpresa.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<ContatoEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ContatosEmpresa.Where(c => c.EmpresaId == empresaId).ToListAsync(ct);

    public async Task<ContatoEmpresa?> GetContatoPrincipalAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ContatosEmpresa.FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.ContatoPrincipal, ct);

    public async Task AddAsync(ContatoEmpresa entity, CancellationToken ct = default)
    {
        _db.ContatosEmpresa.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ContatoEmpresa entity, CancellationToken ct = default)
    {
        _db.ContatosEmpresa.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ContatosEmpresa.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ContatosEmpresa.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class ConfiguracaoGeralRepository : IConfiguracaoGeralRepository
{
    private readonly CadastrosDbContext _db;
    public ConfiguracaoGeralRepository(CadastrosDbContext db) => _db = db;

    public async Task<ConfiguracaoGeral?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ConfiguracoesGerais.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<ConfiguracaoGeral?> GetByChaveAsync(Guid empresaId, string chave, CancellationToken ct = default)
        => await _db.ConfiguracoesGerais.FirstOrDefaultAsync(c => c.EmpresaId == empresaId && c.Chave == chave, ct);

    public async Task<IEnumerable<ConfiguracaoGeral>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ConfiguracoesGerais.Where(c => c.EmpresaId == empresaId).ToListAsync(ct);

    public async Task AddAsync(ConfiguracaoGeral entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesGerais.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ConfiguracaoGeral entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesGerais.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ConfiguracoesGerais.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ConfiguracoesGerais.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class ConfiguracaoESocialRepository : IConfiguracaoESocialRepository
{
    private readonly CadastrosDbContext _db;
    public ConfiguracaoESocialRepository(CadastrosDbContext db) => _db = db;

    public async Task<ConfiguracaoESocial?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ConfiguracoesESocial.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<ConfiguracaoESocial?> GetByEmpresaAsync(Guid empresaId, CancellationToken ct = default)
        => await _db.ConfiguracoesESocial.FirstOrDefaultAsync(c => c.EmpresaId == empresaId, ct);

    public async Task AddAsync(ConfiguracaoESocial entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesESocial.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ConfiguracaoESocial entity, CancellationToken ct = default)
    {
        _db.ConfiguracoesESocial.Update(entity);
        await _db.SaveChangesAsync(ct);
    }
}

// ============================
