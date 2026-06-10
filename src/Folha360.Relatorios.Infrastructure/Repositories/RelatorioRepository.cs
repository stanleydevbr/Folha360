using Folha360.Relatorios.Domain.Abstractions;
using Folha360.Relatorios.Domain.Entities;
using Folha360.Relatorios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Relatorios.Infrastructure.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly RelatoriosDbContext _context;

    public RelatorioRepository(RelatoriosDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ItemFolhaView>> ObterItensFolhaAsync(Guid empresaId, string periodo, CancellationToken ct)
    {
        return await _context.ItemFolhaViews
            .Where(v => v.EmpresaId == empresaId && v.Periodo == periodo)
            .OrderBy(v => v.NomeFuncionario)
            .ThenBy(v => v.CodigoRubrica)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<DirfAnualView>> ObterDirfAnualAsync(Guid empresaId, int ano, CancellationToken ct)
    {
        return await _context.DirfAnualViews
            .Where(v => v.EmpresaId == empresaId && v.Ano == ano)
            .OrderBy(v => v.NomeFuncionario)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RaisAnualView>> ObterRaisAnualAsync(Guid empresaId, int ano, CancellationToken ct)
    {
        return await _context.RaisAnualViews
            .Where(v => v.EmpresaId == empresaId && v.Ano == ano)
            .OrderBy(v => v.NomeFuncionario)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ItemFolhaView>> ObterFolhaAnaliticaAsync(
        Guid empresaId, string periodo, Guid? departamentoId, string? tipoCalculo, CancellationToken ct)
    {
        var query = _context.ItemFolhaViews
            .Where(v => v.EmpresaId == empresaId && v.Periodo == periodo);

        if (departamentoId.HasValue)
            query = query.Where(v => v.DepartamentoId == departamentoId.Value);

        if (!string.IsNullOrEmpty(tipoCalculo))
            query = query.Where(v => v.TipoCalculo == tipoCalculo);

        return await query
            .OrderBy(v => v.NomeFuncionario)
            .ThenBy(v => v.CodigoRubrica)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<string, decimal>> ObterFolhaSinteticaAsync(Guid empresaId, string periodo, CancellationToken ct)
    {
        var dados = await _context.ItemFolhaViews
            .Where(v => v.EmpresaId == empresaId && v.Periodo == periodo)
            .GroupBy(v => new { v.CodigoRubrica, v.NomeRubrica, v.Natureza })
            .Select(g => new
            {
                g.Key.CodigoRubrica,
                g.Key.NomeRubrica,
                g.Key.Natureza,
                Total = g.Sum(v => v.Valor),
            })
            .ToListAsync(ct);

        return dados.ToDictionary(
            d => $"{d.CodigoRubrica}|{d.NomeRubrica}|{d.Natureza}",
            d => d.Total);
    }

    public async Task<Dictionary<string, object>> ObterResumoMensalAsync(Guid empresaId, string periodo, CancellationToken ct)
    {
        var dados = await _context.ItemFolhaViews
            .Where(v => v.EmpresaId == empresaId && v.Periodo == periodo)
            .ToListAsync(ct);

        var totalFuncionarios = dados.Select(v => v.FuncionarioId).Distinct().Count();
        var totalVencimentos = dados.Where(v => v.Natureza == "VENCIMENTO").Sum(v => v.Valor);
        var totalDescontos = dados.Where(v => v.Natureza == "DESCONTO").Sum(v => v.Valor);
        var totalIrrf = dados.Where(v => v.CodigoRubrica == "IRRF").Sum(v => v.Valor);
        var totalInss = dados.Where(v => v.CodigoRubrica == "INSS").Sum(v => v.Valor);
        var totalFgts = dados.Where(v => v.CodigoRubrica == "FGTS").Sum(v => v.Valor);

        return new Dictionary<string, object>
        {
            ["total_funcionarios"] = totalFuncionarios,
            ["total_vencimentos"] = totalVencimentos,
            ["total_descontos"] = Math.Abs(totalDescontos),
            ["total_liquido"] = totalVencimentos - Math.Abs(totalDescontos),
            ["total_irrf"] = Math.Abs(totalIrrf),
            ["total_inss"] = Math.Abs(totalInss),
            ["total_fgts"] = Math.Abs(totalFgts),
        };
    }

    public async Task<IReadOnlyList<Dictionary<string, object>>> ObterResumoAnualAsync(Guid empresaId, int ano, CancellationToken ct)
    {
        var dados = await _context.ItemFolhaViews
            .Where(v => v.EmpresaId == empresaId && v.Periodo.StartsWith(ano.ToString()))
            .GroupBy(v => v.Periodo)
            .Select(g => new
            {
                Periodo = g.Key,
                TotalVencimentos = g.Where(v => v.Natureza == "VENCIMENTO").Sum(v => v.Valor),
                TotalDescontos = g.Where(v => v.Natureza == "DESCONTO").Sum(v => v.Valor),
                TotalFuncionarios = g.Select(v => v.FuncionarioId).Distinct().Count(),
            })
            .OrderBy(g => g.Periodo)
            .ToListAsync(ct);

        return dados.Select(d => new Dictionary<string, object>
        {
            ["periodo"] = d.Periodo,
            ["total_vencimentos"] = d.TotalVencimentos,
            ["total_descontos"] = Math.Abs(d.TotalDescontos),
            ["total_liquido"] = d.TotalVencimentos - Math.Abs(d.TotalDescontos),
            ["total_funcionarios"] = d.TotalFuncionarios,
        }).ToList();
    }
}
