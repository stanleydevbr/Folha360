using Folha360.Application;
using Folha360.Relatorios.Application.DTOs;
using Folha360.Relatorios.Domain.Abstractions;

namespace Folha360.Relatorios.Application.Handlers;

public class ObterDirfHandler : IRequestHandler<ObterDirfQuery, Result<IReadOnlyList<DirfDto>>>
{
    private readonly IRelatorioRepository _repository;

    public ObterDirfHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DirfDto>>> Handle(ObterDirfQuery request, CancellationToken ct)
    {
        var dados = await _repository.ObterDirfAnualAsync(request.EmpresaId, request.Ano, ct);

        var dtos = dados.Select(d => new DirfDto
        {
            FuncionarioId = d.FuncionarioId,
            Cpf = d.Cpf,
            Nome = d.NomeFuncionario,
            RendimentosTributaveis = d.RendimentosTributaveis,
            RendimentosIsentos = d.RendimentosIsentos,
            IrrfRetido = d.IrrfRetido,
            DecimoTerceiro = d.DecimoTerceiro,
            Ferias = d.Ferias,
        }).ToList();

        return Result<IReadOnlyList<DirfDto>>.Success(dtos);
    }
}

public class ObterRaisHandler : IRequestHandler<ObterRaisQuery, Result<IReadOnlyList<RaisDto>>>
{
    private readonly IRelatorioRepository _repository;

    public ObterRaisHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<RaisDto>>> Handle(ObterRaisQuery request, CancellationToken ct)
    {
        var dados = await _repository.ObterRaisAnualAsync(request.EmpresaId, request.Ano, ct);

        var dtos = dados.Select(d => new RaisDto
        {
            FuncionarioId = d.FuncionarioId,
            Cpf = d.Cpf,
            Nome = d.NomeFuncionario,
            PisPasep = d.PisPasep,
            DataAdmissao = d.DataAdmissao,
            DataDesligamento = d.DataDesligamento,
            MotivoDesligamento = d.MotivoDesligamento,
            RemuneracaoJaneiro = d.RemuneracaoJaneiro,
            RemuneracaoFevereiro = d.RemuneracaoFevereiro,
            RemuneracaoMarco = d.RemuneracaoMarco,
            RemuneracaoAbril = d.RemuneracaoAbril,
            RemuneracaoMaio = d.RemuneracaoMaio,
            RemuneracaoJunho = d.RemuneracaoJunho,
            RemuneracaoJulho = d.RemuneracaoJulho,
            RemuneracaoAgosto = d.RemuneracaoAgosto,
            RemuneracaoSetembro = d.RemuneracaoSetembro,
            RemuneracaoOutubro = d.RemuneracaoOutubro,
            RemuneracaoNovembro = d.RemuneracaoNovembro,
            RemuneracaoDezembro = d.RemuneracaoDezembro,
            RemuneracaoTotal = d.RemuneracaoTotal,
            DecimoTerceiro = d.DecimoTerceiro,
        }).ToList();

        return Result<IReadOnlyList<RaisDto>>.Success(dtos);
    }
}

public class ObterFolhaAnaliticaHandler : IRequestHandler<ObterFolhaAnaliticaQuery, Result<FolhaAnaliticaDto>>
{
    private readonly IRelatorioRepository _repository;

    public ObterFolhaAnaliticaHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FolhaAnaliticaDto>> Handle(ObterFolhaAnaliticaQuery request, CancellationToken ct)
    {
        var itens = await _repository.ObterFolhaAnaliticaAsync(
            request.EmpresaId, request.Periodo, request.DepartamentoId, request.TipoCalculo, ct);

        var funcionarios = itens.GroupBy(i => i.FuncionarioId).Select(g => new FuncionarioFolhaDto
        {
            FuncionarioId = g.Key,
            Nome = g.First().NomeFuncionario,
            Departamento = g.First().NomeDepartamento,
            Vencimentos = g.Where(i => i.Natureza == "VENCIMENTO").Select(i => new RubricaItemDto
            {
                Codigo = i.CodigoRubrica,
                Nome = i.NomeRubrica,
                Valor = i.Valor,
            }).ToList(),
            Descontos = g.Where(i => i.Natureza == "DESCONTO").Select(i => new RubricaItemDto
            {
                Codigo = i.CodigoRubrica,
                Nome = i.NomeRubrica,
                Valor = Math.Abs(i.Valor),
            }).ToList(),
            TotalVencimentos = g.Where(i => i.Natureza == "VENCIMENTO").Sum(i => i.Valor),
            TotalDescontos = Math.Abs(g.Where(i => i.Natureza == "DESCONTO").Sum(i => i.Valor)),
            Liquido = g.Sum(i => i.Natureza == "VENCIMENTO" ? i.Valor : -i.Valor),
        }).ToList();

        // Paginação
        var total = funcionarios.Count;
        var paged = funcionarios.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

        return Result<FolhaAnaliticaDto>.Success(new FolhaAnaliticaDto
        {
            EmpresaId = request.EmpresaId,
            Periodo = request.Periodo,
            Funcionarios = paged,
        });
    }
}

public class ObterFolhaSinteticaHandler : IRequestHandler<ObterFolhaSinteticaQuery, Result<FolhaSinteticaDto>>
{
    private readonly IRelatorioRepository _repository;

    public ObterFolhaSinteticaHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FolhaSinteticaDto>> Handle(ObterFolhaSinteticaQuery request, CancellationToken ct)
    {
        var totais = await _repository.ObterFolhaSinteticaAsync(request.EmpresaId, request.Periodo, ct);

        return Result<FolhaSinteticaDto>.Success(new FolhaSinteticaDto
        {
            EmpresaId = request.EmpresaId,
            Periodo = request.Periodo,
            TotaisPorRubrica = totais.Select(t => new RubricaTotalDto
            {
                Codigo = t.CodigoRubrica,
                Nome = t.NomeRubrica,
                Natureza = t.Natureza,
                Valor = t.Total,
            }).ToList(),
        });
    }
}

public class ObterResumoMensalHandler : IRequestHandler<ObterResumoMensalQuery, Result<ResumoMensalDto>>
{
    private readonly IRelatorioRepository _repository;

    public ObterResumoMensalHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ResumoMensalDto>> Handle(ObterResumoMensalQuery request, CancellationToken ct)
    {
        var dados = await _repository.ObterResumoMensalAsync(request.EmpresaId, request.Periodo, ct);

        return Result<ResumoMensalDto>.Success(new ResumoMensalDto
        {
            EmpresaId = request.EmpresaId,
            Periodo = request.Periodo,
            TotalFuncionarios = dados.TotalFuncionarios,
            TotalVencimentos = dados.TotalVencimentos,
            TotalDescontos = dados.TotalDescontos,
            TotalLiquido = dados.TotalLiquido,
            TotalIrrf = dados.TotalIrrf,
            TotalInss = dados.TotalInss,
            TotalFgts = dados.TotalFgts,
        });
    }
}

public class ObterResumoAnualHandler : IRequestHandler<ObterResumoAnualQuery, Result<ResumoAnualDto>>
{
    private readonly IRelatorioRepository _repository;

    public ObterResumoAnualHandler(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ResumoAnualDto>> Handle(ObterResumoAnualQuery request, CancellationToken ct)
    {
        var dados = await _repository.ObterResumoAnualAsync(request.EmpresaId, request.Ano, ct);

        var meses = dados.Select(d => new ResumoMensalDto
        {
            EmpresaId = request.EmpresaId,
            Periodo = d.Periodo,
            TotalVencimentos = d.TotalVencimentos,
            TotalDescontos = d.TotalDescontos,
            TotalLiquido = d.TotalLiquido,
            TotalFuncionarios = d.TotalFuncionarios,
        }).ToList();

        return Result<ResumoAnualDto>.Success(new ResumoAnualDto
        {
            EmpresaId = request.EmpresaId,
            Ano = request.Ano,
            Meses = meses,
            TotalAnualVencimentos = meses.Sum(m => m.TotalVencimentos),
            TotalAnualDescontos = meses.Sum(m => m.TotalDescontos),
            TotalAnualLiquido = meses.Sum(m => m.TotalLiquido),
            MediaMensalVencimentos = meses.Count > 0 ? meses.Average(m => m.TotalVencimentos) : 0,
        });
    }
}
