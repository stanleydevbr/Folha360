using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Repositories;

// ============================
// Employee Expansion (T13)
// ============================
// Employee Expansion (T13)
// ============================
public class DadosBancariosFuncionarioRepository : IDadosBancariosFuncionarioRepository
{
    private readonly CadastrosDbContext _db;
    public DadosBancariosFuncionarioRepository(CadastrosDbContext db) => _db = db;

    public async Task<DadosBancariosFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.DadosBancariosFuncionarios.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<IEnumerable<DadosBancariosFuncionario>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.DadosBancariosFuncionarios.Where(d => d.FuncionarioId == funcionarioId).ToListAsync(ct);

    public async Task<DadosBancariosFuncionario?> GetContaPrincipalAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.DadosBancariosFuncionarios.FirstOrDefaultAsync(d => d.FuncionarioId == funcionarioId && d.ContaPrincipal, ct);

    public async Task AddAsync(DadosBancariosFuncionario entity, CancellationToken ct = default)
    {
        _db.DadosBancariosFuncionarios.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DadosBancariosFuncionario entity, CancellationToken ct = default)
    {
        _db.DadosBancariosFuncionarios.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.DadosBancariosFuncionarios.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.DadosBancariosFuncionarios.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class ContratoTrabalhoRepository : IContratoTrabalhoRepository
{
    private readonly CadastrosDbContext _db;
    public ContratoTrabalhoRepository(CadastrosDbContext db) => _db = db;

    public async Task<ContratoTrabalho?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.ContratosTrabalho.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<ContratoTrabalho?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.ContratosTrabalho.FirstOrDefaultAsync(c => c.FuncionarioId == funcionarioId, ct);

    public async Task AddAsync(ContratoTrabalho entity, CancellationToken ct = default)
    {
        _db.ContratosTrabalho.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ContratoTrabalho entity, CancellationToken ct = default)
    {
        _db.ContratosTrabalho.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.ContratosTrabalho.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.ContratosTrabalho.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class RemuneracaoBeneficioRepository : IRemuneracaoBeneficioRepository
{
    private readonly CadastrosDbContext _db;
    public RemuneracaoBeneficioRepository(CadastrosDbContext db) => _db = db;

    public async Task<RemuneracaoBeneficio?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.RemuneracoesBeneficios.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<RemuneracaoBeneficio?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.RemuneracoesBeneficios.FirstOrDefaultAsync(r => r.FuncionarioId == funcionarioId, ct);

    public async Task AddAsync(RemuneracaoBeneficio entity, CancellationToken ct = default)
    {
        _db.RemuneracoesBeneficios.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RemuneracaoBeneficio entity, CancellationToken ct = default)
    {
        _db.RemuneracoesBeneficios.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.RemuneracoesBeneficios.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.RemuneracoesBeneficios.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class AfastamentoRepository : IAfastamentoFuncionarioRepository
{
    private readonly CadastrosDbContext _db;
    public AfastamentoRepository(CadastrosDbContext db) => _db = db;

    public async Task<AfastamentoFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Afastamentos.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<AfastamentoFuncionario>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.Afastamentos.Where(a => a.FuncionarioId == funcionarioId).OrderByDescending(a => a.DataInicio).ToListAsync(ct);

    public async Task AddAsync(AfastamentoFuncionario entity, CancellationToken ct = default)
    {
        _db.Afastamentos.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AfastamentoFuncionario entity, CancellationToken ct = default)
    {
        _db.Afastamentos.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Afastamentos.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.Afastamentos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class InfoESocialFuncionarioRepository : IInfoESocialFuncionarioRepository
{
    private readonly CadastrosDbContext _db;
    public InfoESocialFuncionarioRepository(CadastrosDbContext db) => _db = db;

    public async Task<InfoESocialFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.InfosESocialFuncionario.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<InfoESocialFuncionario?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.InfosESocialFuncionario.FirstOrDefaultAsync(i => i.FuncionarioId == funcionarioId, ct);

    public async Task AddAsync(InfoESocialFuncionario entity, CancellationToken ct = default)
    {
        _db.InfosESocialFuncionario.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(InfoESocialFuncionario entity, CancellationToken ct = default)
    {
        _db.InfosESocialFuncionario.Update(entity);
        await _db.SaveChangesAsync(ct);
    }
}

public class MovimentacaoFixaRepository : IMovimentacaoFixaRepository
{
    private readonly CadastrosDbContext _db;
    public MovimentacaoFixaRepository(CadastrosDbContext db) => _db = db;

    public async Task<MovimentacaoFixa?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.MovimentacoesFixas.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IEnumerable<MovimentacaoFixa>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default)
        => await _db.MovimentacoesFixas.Where(m => m.FuncionarioId == funcionarioId).ToListAsync(ct);

    public async Task AddAsync(MovimentacaoFixa entity, CancellationToken ct = default)
    {
        _db.MovimentacoesFixas.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MovimentacaoFixa entity, CancellationToken ct = default)
    {
        _db.MovimentacoesFixas.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.MovimentacoesFixas.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.MovimentacoesFixas.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

public class MovimentacaoMensalRepository : IMovimentacaoMensalRepository
{
    private readonly CadastrosDbContext _db;
    public MovimentacaoMensalRepository(CadastrosDbContext db) => _db = db;

    public async Task<MovimentacaoMensal?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.MovimentacoesMensais.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IEnumerable<MovimentacaoMensal>> ListarPorFuncionarioMesAsync(Guid funcionarioId, string mesAno, CancellationToken ct = default)
        => await _db.MovimentacoesMensais.Where(m => m.FuncionarioId == funcionarioId && m.MesAno == mesAno).ToListAsync(ct);

    public async Task<MovimentacaoMensal?> GetByFuncionarioRubricaMesAsync(Guid funcionarioId, Guid rubricaId, string mesAno, CancellationToken ct = default)
        => await _db.MovimentacoesMensais.FirstOrDefaultAsync(m => m.FuncionarioId == funcionarioId && m.RubricaId == rubricaId && m.MesAno == mesAno, ct);

    public async Task AddAsync(MovimentacaoMensal entity, CancellationToken ct = default)
    {
        _db.MovimentacoesMensais.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(MovimentacaoMensal entity, CancellationToken ct = default)
    {
        _db.MovimentacoesMensais.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.MovimentacoesMensais.FindAsync([id], ct);
        if (entity is not null)
        {
            _db.MovimentacoesMensais.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
