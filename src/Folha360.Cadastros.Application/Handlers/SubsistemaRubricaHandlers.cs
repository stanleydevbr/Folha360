using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Domain.Abstractions;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

// ============================
// GrupoRubrica Handlers
// ============================
public class CriarGrupoRubricaHandler : IRequestHandler<CriarGrupoRubricaCommand, Result<GrupoRubricaDto>>
{
    private readonly IGrupoRubricaRepository _repo;

    public CriarGrupoRubricaHandler(IGrupoRubricaRepository repo) => _repo = repo;

    public async Task<Result<GrupoRubricaDto>> Handle(CriarGrupoRubricaCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<GrupoRubricaDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");

        var existente = await _repo.GetByCodigoAsync(cmd.EmpresaId, cmd.Codigo, ct);
        if (existente is not null)
            return Result<GrupoRubricaDto>.Failure("CODIGO_DUPLICADO", "Já existe um grupo com este código.");

        var grupo = new GrupoRubrica(cmd.EmpresaId, cmd.Codigo, cmd.Descricao, cmd.Natureza, cmd.OrdemExibicao);
        await _repo.AddAsync(grupo, ct);

        return Result<GrupoRubricaDto>.Success(Map(grupo));
    }

    private static GrupoRubricaDto Map(GrupoRubrica g) => new()
    {
        Id = g.Id,
        EmpresaId = g.EmpresaId,
        Codigo = g.Codigo,
        Descricao = g.Descricao,
        Natureza = g.Natureza,
        OrdemExibicao = g.OrdemExibicao,
        CreatedAt = g.CreatedAt,
        UpdatedAt = g.UpdatedAt,
    };
}

public class AtualizarGrupoRubricaHandler : IRequestHandler<AtualizarGrupoRubricaCommand, Result<GrupoRubricaDto>>
{
    private readonly IGrupoRubricaRepository _repo;

    public AtualizarGrupoRubricaHandler(IGrupoRubricaRepository repo) => _repo = repo;

    public async Task<Result<GrupoRubricaDto>> Handle(AtualizarGrupoRubricaCommand cmd, CancellationToken ct)
    {
        var grupo = await _repo.GetByIdAsync(cmd.Id, ct);
        if (grupo is null)
            return Result<GrupoRubricaDto>.Failure("NAO_ENCONTRADO", "Grupo não encontrado.");

        grupo.Atualizar(cmd.Descricao, cmd.Natureza, cmd.OrdemExibicao);
        await _repo.UpdateAsync(grupo, ct);

        return Result<GrupoRubricaDto>.Success(new GrupoRubricaDto
        {
            Id = grupo.Id,
            EmpresaId = grupo.EmpresaId,
            Codigo = grupo.Codigo,
            Descricao = grupo.Descricao,
            Natureza = grupo.Natureza,
            OrdemExibicao = grupo.OrdemExibicao,
            CreatedAt = grupo.CreatedAt,
            UpdatedAt = grupo.UpdatedAt,
        });
    }
}

public class ExcluirGrupoRubricaHandler : IRequestHandler<ExcluirGrupoRubricaCommand, Result<bool>>
{
    private readonly IGrupoRubricaRepository _repo;

    public ExcluirGrupoRubricaHandler(IGrupoRubricaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirGrupoRubricaCommand cmd, CancellationToken ct)
    {
        if (await _repo.HasRubricasVinculadasAsync(cmd.Id, ct))
            return Result<bool>.Failure("VINCULO_ATIVO", "Não é possível excluir: existem rubricas vinculadas a este grupo.");

        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterGrupoRubricaHandler : IRequestHandler<ObterGrupoRubricaQuery, Result<GrupoRubricaDto>>
{
    private readonly IGrupoRubricaRepository _repo;

    public ObterGrupoRubricaHandler(IGrupoRubricaRepository repo) => _repo = repo;

    public async Task<Result<GrupoRubricaDto>> Handle(ObterGrupoRubricaQuery query, CancellationToken ct)
    {
        var g = await _repo.GetByIdAsync(query.Id, ct);
        if (g is null)
            return Result<GrupoRubricaDto>.Failure("NAO_ENCONTRADO", "Grupo não encontrado.");

        return Result<GrupoRubricaDto>.Success(new GrupoRubricaDto
        {
            Id = g.Id,
            EmpresaId = g.EmpresaId,
            Codigo = g.Codigo,
            Descricao = g.Descricao,
            Natureza = g.Natureza,
            OrdemExibicao = g.OrdemExibicao,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt,
        });
    }
}

public class ListarGruposRubricaHandler : IRequestHandler<ListarGruposRubricaQuery, PaginatedResult<GrupoRubricaDto>>
{
    private readonly IGrupoRubricaRepository _repo;

    public ListarGruposRubricaHandler(IGrupoRubricaRepository repo) => _repo = repo;

    public async Task<PaginatedResult<GrupoRubricaDto>> Handle(ListarGruposRubricaQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy,
            query.EmpresaId, query.Natureza, ct);

        var dtos = items.Select(g => new GrupoRubricaDto
        {
            Id = g.Id,
            EmpresaId = g.EmpresaId,
            Codigo = g.Codigo,
            Descricao = g.Descricao,
            Natureza = g.Natureza,
            OrdemExibicao = g.OrdemExibicao,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt,
        });

        return PaginatedResult<GrupoRubricaDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}

// ============================
// RubricaComposicao Handlers
// ============================
public class AdicionarComponenteHandler : IRequestHandler<AdicionarComponenteCommand, Result<RubricaComposicaoDto>>
{
    private readonly IRubricaComposicaoRepository _repo;
    private readonly IRubricaRepository _rubricaRepo;

    public AdicionarComponenteHandler(IRubricaComposicaoRepository repo, IRubricaRepository rubricaRepo)
    {
        _repo = repo;
        _rubricaRepo = rubricaRepo;
    }

    public async Task<Result<RubricaComposicaoDto>> Handle(AdicionarComponenteCommand cmd, CancellationToken ct)
    {
        if (cmd.RubricaPrincipalId == cmd.RubricaComponenteId)
            return Result<RubricaComposicaoDto>.Failure("VALIDACAO", "Uma rubrica não pode ser componente de si mesma.");

        // Verifica se a rubrica principal tem tipo_calculo = COMPOSICAO
        var principal = await _rubricaRepo.GetByIdAsync(cmd.RubricaPrincipalId, ct);
        if (principal is null)
            return Result<RubricaComposicaoDto>.Failure("NAO_ENCONTRADO", "Rubrica principal não encontrada.");
        if (principal.TipoCalculo != "COMPOSICAO")
        {
            return Result<RubricaComposicaoDto>.Failure("VALIDACAO", "A rubrica principal deve ter tipo_calculo = 'COMPOSICAO'.");
        }

        // Detecção de ciclo: verifica se já existe caminho do componente para o principal
        if (await _repo.ExistsCaminhoAsync(cmd.RubricaComponenteId, cmd.RubricaPrincipalId, ct))
        {
            return Result<RubricaComposicaoDto>.Failure("CICLO_DETECTADO",
                $"A composição criaria um ciclo. A rubrica componente já depende (direta ou indiretamente) da rubrica principal.");
        }

        var composicao = new RubricaComposicao(
            cmd.RubricaPrincipalId, cmd.RubricaComponenteId,
            cmd.Operador, cmd.PercentualComposicao, cmd.Ordem, cmd.Obrigatorio);

        await _repo.AddAsync(composicao, ct);

        return Result<RubricaComposicaoDto>.Success(new RubricaComposicaoDto
        {
            Id = composicao.Id,
            RubricaPrincipalId = composicao.RubricaPrincipalId,
            RubricaComponenteId = composicao.RubricaComponenteId,
            Operador = composicao.Operador,
            PercentualComposicao = composicao.PercentualComposicao,
            Ordem = composicao.Ordem,
            Obrigatorio = composicao.Obrigatorio,
            CreatedAt = composicao.CreatedAt,
            UpdatedAt = composicao.UpdatedAt,
        });
    }
}

public class RemoverComponenteHandler : IRequestHandler<RemoverComponenteCommand, Result<bool>>
{
    private readonly IRubricaComposicaoRepository _repo;

    public RemoverComponenteHandler(IRubricaComposicaoRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(RemoverComponenteCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteAsync(cmd.ComposicaoId, ct);
        return Result<bool>.Success(true);
    }
}

public class ListarComposicaoHandler : IRequestHandler<ListarComposicaoQuery, Result<List<RubricaComposicaoDto>>>
{
    private readonly IRubricaComposicaoRepository _repo;
    private readonly IRubricaRepository _rubricaRepo;

    public ListarComposicaoHandler(IRubricaComposicaoRepository repo, IRubricaRepository rubricaRepo)
    {
        _repo = repo;
        _rubricaRepo = rubricaRepo;
    }

    public async Task<Result<List<RubricaComposicaoDto>>> Handle(ListarComposicaoQuery query, CancellationToken ct)
    {
        var composicoes = await _repo.GetByPrincipalAsync(query.RubricaPrincipalId, ct);

        var dtos = new List<RubricaComposicaoDto>();
        foreach (var c in composicoes)
        {
            var principal = await _rubricaRepo.GetByIdAsync(c.RubricaPrincipalId, ct);
            var componente = await _rubricaRepo.GetByIdAsync(c.RubricaComponenteId, ct);

            dtos.Add(new RubricaComposicaoDto
            {
                Id = c.Id,
                RubricaPrincipalId = c.RubricaPrincipalId,
                RubricaComponenteId = c.RubricaComponenteId,
                Operador = c.Operador,
                PercentualComposicao = c.PercentualComposicao,
                Ordem = c.Ordem,
                Obrigatorio = c.Obrigatorio,
                RubricaPrincipalCodigo = principal?.Codigo,
                RubricaComponenteCodigo = componente?.Codigo,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            });
        }

        return Result<List<RubricaComposicaoDto>>.Success(dtos);
    }
}

// ============================
// RubricaFormula Handlers
// ============================
public class CriarRubricaFormulaHandler : IRequestHandler<CriarRubricaFormulaCommand, Result<RubricaFormulaDto>>
{
    private readonly IRubricaFormulaRepository _repo;

    public CriarRubricaFormulaHandler(IRubricaFormulaRepository repo) => _repo = repo;

    public async Task<Result<RubricaFormulaDto>> Handle(CriarRubricaFormulaCommand cmd, CancellationToken ct)
    {
        var existente = await _repo.GetByRubricaAsync(cmd.RubricaId, ct);
        if (existente is not null)
            return Result<RubricaFormulaDto>.Failure("FORMULA_EXISTENTE", "Esta rubrica já possui uma fórmula. Use o endpoint de atualização.");

        if (!ValidarExpressao(cmd.Expressao, out var erro))
            return Result<RubricaFormulaDto>.Failure("SINTAXE_INVALIDA", erro);

        var formula = new RubricaFormula(cmd.RubricaId, cmd.Expressao, cmd.Parametros, cmd.DescricaoFormal);
        await _repo.AddAsync(formula, ct);

        return Result<RubricaFormulaDto>.Success(Map(formula));
    }

    private static RubricaFormulaDto Map(RubricaFormula f) => new()
    {
        Id = f.Id,
        RubricaId = f.RubricaId,
        Expressao = f.Expressao,
        Parametros = f.Parametros,
        DescricaoFormal = f.DescricaoFormal,
        Versao = f.Versao,
        CreatedAt = f.CreatedAt,
        UpdatedAt = f.UpdatedAt,
    };

    private static bool ValidarExpressao(string expressao, out string erro)
    {
        erro = string.Empty;
        if (string.IsNullOrWhiteSpace(expressao))
        {
            erro = "A expressão não pode ser vazia.";
            return false;
        }

        try
        {
            // Validação básica de sintaxe com NCalc
            var expr = new NCalc.Expression(expressao.Replace("{", "[").Replace("}", "]"));
            if (expr.HasErrors())
            {
                erro = expr.Error ?? "Erro de sintaxe na expressão.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            erro = $"Erro ao validar expressão: {ex.Message}";
            return false;
        }
    }
}

public class AtualizarRubricaFormulaHandler : IRequestHandler<AtualizarRubricaFormulaCommand, Result<RubricaFormulaDto>>
{
    private readonly IRubricaFormulaRepository _repo;

    public AtualizarRubricaFormulaHandler(IRubricaFormulaRepository repo) => _repo = repo;

    public async Task<Result<RubricaFormulaDto>> Handle(AtualizarRubricaFormulaCommand cmd, CancellationToken ct)
    {
        var formula = await _repo.GetByRubricaAsync(cmd.RubricaId, ct);
        if (formula is null)
            return Result<RubricaFormulaDto>.Failure("NAO_ENCONTRADO", "Fórmula não encontrada para esta rubrica.");

        if (string.IsNullOrWhiteSpace(cmd.Expressao))
            return Result<RubricaFormulaDto>.Failure("SINTAXE_INVALIDA", "A expressão não pode ser vazia.");

        formula.Atualizar(cmd.Expressao, cmd.Parametros, cmd.DescricaoFormal);
        await _repo.UpdateAsync(formula, ct);

        return Result<RubricaFormulaDto>.Success(new RubricaFormulaDto
        {
            Id = formula.Id,
            RubricaId = formula.RubricaId,
            Expressao = formula.Expressao,
            Parametros = formula.Parametros,
            DescricaoFormal = formula.DescricaoFormal,
            Versao = formula.Versao,
            CreatedAt = formula.CreatedAt,
            UpdatedAt = formula.UpdatedAt,
        });
    }
}

public class ObterRubricaFormulaHandler : IRequestHandler<ObterRubricaFormulaQuery, Result<RubricaFormulaDto>>
{
    private readonly IRubricaFormulaRepository _repo;

    public ObterRubricaFormulaHandler(IRubricaFormulaRepository repo) => _repo = repo;

    public async Task<Result<RubricaFormulaDto>> Handle(ObterRubricaFormulaQuery query, CancellationToken ct)
    {
        var f = await _repo.GetByRubricaAsync(query.RubricaId, ct);
        if (f is null)
            return Result<RubricaFormulaDto>.Failure("NAO_ENCONTRADO", "Fórmula não encontrada para esta rubrica.");

        return Result<RubricaFormulaDto>.Success(new RubricaFormulaDto
        {
            Id = f.Id,
            RubricaId = f.RubricaId,
            Expressao = f.Expressao,
            Parametros = f.Parametros,
            DescricaoFormal = f.DescricaoFormal,
            Versao = f.Versao,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt,
        });
    }
}

// ============================
// RubricaTabelaProgressiva Handlers
// ============================
public class CriarFaixaProgressivaHandler : IRequestHandler<CriarFaixaProgressivaCommand, Result<RubricaTabelaProgressivaDto>>
{
    private readonly IRubricaTabelaProgressivaRepository _repo;
    private readonly IMessageBus _messageBus;

    public CriarFaixaProgressivaHandler(IRubricaTabelaProgressivaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaTabelaProgressivaDto>> Handle(CriarFaixaProgressivaCommand cmd, CancellationToken ct)
    {
        if (cmd.Aliquota < 0 || cmd.Aliquota > 100)
            return Result<RubricaTabelaProgressivaDto>.Failure("VALIDACAO", "Alíquota deve estar entre 0 e 100.");

        if (cmd.FaixaAte.HasValue && cmd.FaixaAte.Value <= cmd.FaixaDe)
            return Result<RubricaTabelaProgressivaDto>.Failure("VALIDACAO", "FaixaAte deve ser maior que FaixaDe.");

        if (await _repo.HasSobreposicaoAsync(cmd.RubricaId, cmd.AnoVigencia, cmd.FaixaDe, cmd.FaixaAte, ct: ct))
            return Result<RubricaTabelaProgressivaDto>.Failure("SOBREPOSICAO", "A faixa se sobrepõe a uma faixa existente para o mesmo ano.");

        var faixa = new RubricaTabelaProgressiva(cmd.RubricaId, cmd.AnoVigencia, cmd.FaixaDe, cmd.FaixaAte, cmd.Aliquota, cmd.Deducao, cmd.Ordem);
        await _repo.AddAsync(faixa, ct);

        await _messageBus.PublishAsync(new { faixa.RubricaId, faixa.AnoVigencia },
            "folha360.cadastros", "TabelaProgressivaAtualizada", ct);

        return Result<RubricaTabelaProgressivaDto>.Success(Map(faixa));
    }

    private static RubricaTabelaProgressivaDto Map(RubricaTabelaProgressiva t) => new()
    {
        Id = t.Id,
        RubricaId = t.RubricaId,
        AnoVigencia = t.AnoVigencia,
        FaixaDe = t.FaixaDe,
        FaixaAte = t.FaixaAte,
        Aliquota = t.Aliquota,
        Deducao = t.Deducao,
        Ordem = t.Ordem,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt,
    };
}

public class AtualizarFaixaProgressivaHandler : IRequestHandler<AtualizarFaixaProgressivaCommand, Result<RubricaTabelaProgressivaDto>>
{
    private readonly IRubricaTabelaProgressivaRepository _repo;
    private readonly IMessageBus _messageBus;

    public AtualizarFaixaProgressivaHandler(IRubricaTabelaProgressivaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaTabelaProgressivaDto>> Handle(AtualizarFaixaProgressivaCommand cmd, CancellationToken ct)
    {
        var faixa = await _repo.GetByIdAsync(cmd.Id, ct);
        if (faixa is null)
            return Result<RubricaTabelaProgressivaDto>.Failure("NAO_ENCONTRADO", "Faixa não encontrada.");

        if (cmd.Aliquota < 0 || cmd.Aliquota > 100)
            return Result<RubricaTabelaProgressivaDto>.Failure("VALIDACAO", "Alíquota deve estar entre 0 e 100.");

        if (cmd.FaixaAte.HasValue && cmd.FaixaAte.Value <= cmd.FaixaDe)
            return Result<RubricaTabelaProgressivaDto>.Failure("VALIDACAO", "FaixaAte deve ser maior que FaixaDe.");

        if (await _repo.HasSobreposicaoAsync(faixa.RubricaId, cmd.AnoVigencia, cmd.FaixaDe, cmd.FaixaAte, cmd.Id, ct))
            return Result<RubricaTabelaProgressivaDto>.Failure("SOBREPOSICAO", "A faixa se sobrepõe a uma faixa existente para o mesmo ano.");

        // Atualiza propriedades via reflection ou método público — RubricaTabelaProgressiva não tem método Atualizar,
        // então vamos criar uma nova e substituir
        var novaFaixa = new RubricaTabelaProgressiva(faixa.RubricaId, cmd.AnoVigencia, cmd.FaixaDe, cmd.FaixaAte, cmd.Aliquota, cmd.Deducao, cmd.Ordem);
        await _repo.DeleteAsync(cmd.Id, ct);
        await _repo.AddAsync(novaFaixa, ct);

        await _messageBus.PublishAsync(new { novaFaixa.RubricaId, novaFaixa.AnoVigencia },
            "folha360.cadastros", "TabelaProgressivaAtualizada", ct);

        return Result<RubricaTabelaProgressivaDto>.Success(new RubricaTabelaProgressivaDto
        {
            Id = novaFaixa.Id,
            RubricaId = novaFaixa.RubricaId,
            AnoVigencia = novaFaixa.AnoVigencia,
            FaixaDe = novaFaixa.FaixaDe,
            FaixaAte = novaFaixa.FaixaAte,
            Aliquota = novaFaixa.Aliquota,
            Deducao = novaFaixa.Deducao,
            Ordem = novaFaixa.Ordem,
            CreatedAt = novaFaixa.CreatedAt,
            UpdatedAt = novaFaixa.UpdatedAt,
        });
    }
}

public class ExcluirFaixaProgressivaHandler : IRequestHandler<ExcluirFaixaProgressivaCommand, Result<bool>>
{
    private readonly IRubricaTabelaProgressivaRepository _repo;

    public ExcluirFaixaProgressivaHandler(IRubricaTabelaProgressivaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirFaixaProgressivaCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ListarFaixasProgressivasHandler : IRequestHandler<ListarFaixasProgressivasQuery, Result<List<RubricaTabelaProgressivaDto>>>
{
    private readonly IRubricaTabelaProgressivaRepository _repo;

    public ListarFaixasProgressivasHandler(IRubricaTabelaProgressivaRepository repo) => _repo = repo;

    public async Task<Result<List<RubricaTabelaProgressivaDto>>> Handle(ListarFaixasProgressivasQuery query, CancellationToken ct)
    {
        IEnumerable<RubricaTabelaProgressiva> faixas;

        if (query.AnoVigencia.HasValue)
            faixas = await _repo.GetByAnoVigenciaAsync(query.RubricaId, query.AnoVigencia.Value, ct);
        else
            faixas = await _repo.GetByRubricaAsync(query.RubricaId, ct);

        var dtos = faixas.Select(t => new RubricaTabelaProgressivaDto
        {
            Id = t.Id,
            RubricaId = t.RubricaId,
            AnoVigencia = t.AnoVigencia,
            FaixaDe = t.FaixaDe,
            FaixaAte = t.FaixaAte,
            Aliquota = t.Aliquota,
            Deducao = t.Deducao,
            Ordem = t.Ordem,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
        }).ToList();

        return Result<List<RubricaTabelaProgressivaDto>>.Success(dtos);
    }
}

// ============================
// RubricaHistorico Handler
// ============================
public class ListarHistoricoRubricaHandler : IRequestHandler<ListarHistoricoRubricaQuery, PaginatedResult<RubricaHistoricoDto>>
{
    private readonly IRubricaHistoricoRepository _repo;

    public ListarHistoricoRubricaHandler(IRubricaHistoricoRepository repo) => _repo = repo;

    public async Task<PaginatedResult<RubricaHistoricoDto>> Handle(ListarHistoricoRubricaQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetByRubricaPagedAsync(query.RubricaId, query.Page, query.PageSize, ct);

        var dtos = items.Select(h => new RubricaHistoricoDto
        {
            Id = h.Id,
            RubricaId = h.RubricaId,
            DadosAnteriores = h.DadosAnteriores,
            DadosNovos = h.DadosNovos,
            Motivo = h.Motivo,
            UsuarioId = h.UsuarioId,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt,
        });

        return PaginatedResult<RubricaHistoricoDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}

// ============================
// RubricaIncidencia Handlers
// ============================
public class AdicionarIncidenciaHandler : IRequestHandler<AdicionarIncidenciaCommand, Result<RubricaIncidenciaDto>>
{
    private readonly IRubricaIncidenciaRepository _repo;

    public AdicionarIncidenciaHandler(IRubricaIncidenciaRepository repo) => _repo = repo;

    public async Task<Result<RubricaIncidenciaDto>> Handle(AdicionarIncidenciaCommand cmd, CancellationToken ct)
    {
        if (await _repo.ExistsAsync(cmd.RubricaId, cmd.TipoIncidencia, ct))
            return Result<RubricaIncidenciaDto>.Failure("DUPLICADO", "Esta incidência já está configurada para esta rubrica.");

        var incidencia = new RubricaIncidencia(cmd.RubricaId, cmd.TipoIncidencia);
        await _repo.AddAsync(incidencia, ct);

        return Result<RubricaIncidenciaDto>.Success(new RubricaIncidenciaDto
        {
            Id = incidencia.Id,
            RubricaId = incidencia.RubricaId,
            TipoIncidencia = incidencia.TipoIncidencia,
            CreatedAt = incidencia.CreatedAt,
            UpdatedAt = incidencia.UpdatedAt,
        });
    }
}

public class RemoverIncidenciaHandler : IRequestHandler<RemoverIncidenciaCommand, Result<bool>>
{
    private readonly IRubricaIncidenciaRepository _repo;

    public RemoverIncidenciaHandler(IRubricaIncidenciaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(RemoverIncidenciaCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteAsync(cmd.IncidenciaId, ct);
        return Result<bool>.Success(true);
    }
}

public class ListarIncidenciasHandler : IRequestHandler<ListarIncidenciasQuery, Result<List<RubricaIncidenciaDto>>>
{
    private readonly IRubricaIncidenciaRepository _repo;

    public ListarIncidenciasHandler(IRubricaIncidenciaRepository repo) => _repo = repo;

    public async Task<Result<List<RubricaIncidenciaDto>>> Handle(ListarIncidenciasQuery query, CancellationToken ct)
    {
        var incidencias = await _repo.GetByRubricaAsync(query.RubricaId, ct);

        var dtos = incidencias.Select(i => new RubricaIncidenciaDto
        {
            Id = i.Id,
            RubricaId = i.RubricaId,
            TipoIncidencia = i.TipoIncidencia,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt,
        }).ToList();

        return Result<List<RubricaIncidenciaDto>>.Success(dtos);
    }
}
