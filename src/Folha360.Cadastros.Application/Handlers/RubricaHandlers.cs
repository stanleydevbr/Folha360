using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Events;
using Folha360.Domain.Abstractions;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class CriarRubricaHandler : IRequestHandler<CriarRubricaCommand, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    private readonly IMessageBus _messageBus;

    public CriarRubricaHandler(IRubricaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaDto>> Handle(CriarRubricaCommand cmd, CancellationToken ct)
    {
        if (cmd.EmpresaId == Guid.Empty)
            return Result<RubricaDto>.Failure("VALIDACAO", "EmpresaId é obrigatório.");

        var existente = await _repo.GetByCodigoAsync(cmd.EmpresaId, cmd.Codigo, ct);
        if (existente is not null)
            return Result<RubricaDto>.Failure("CODIGO_DUPLICADO", "Já existe uma rubrica com este código.");

        var rubrica = new Rubrica(
            empresaId: cmd.EmpresaId,
            codigo: cmd.Codigo,
            descricao: cmd.Descricao,
            natureza: cmd.Natureza,
            tipoEsocial: cmd.TipoEsocial,
            descricaoAbreviada: cmd.DescricaoAbreviada,
            enviarEsocial: cmd.EnviarEsocial,
            incideInss: cmd.IncideInss,
            incideIrrf: cmd.IncideIrrf,
            incideFgts: cmd.IncideFgts,
            incideContribuicaoSindical: cmd.IncideContribuicaoSindical,
            incideDecimoTerceiro: cmd.IncideDecimoTerceiro,
            incideFerias: cmd.IncideFerias,
            incideAvisoPrevio: cmd.IncideAvisoPrevio,
            incideRescisao: cmd.IncideRescisao,
            incideDissidio: cmd.IncideDissidio,
            incideSalarioMaternidade: cmd.IncideSalarioMaternidade,
            incideAuxilioDoenca: cmd.IncideAuxilioDoenca,
            incideAdiantamento: cmd.IncideAdiantamento,
            tipoCalculo: cmd.TipoCalculo,
            formulaCalculo: cmd.FormulaCalculo,
            valorFixo: cmd.ValorFixo,
            percentual: cmd.Percentual,
            rubricaBaseId: cmd.RubricaBaseId,
            ordemCalculo: cmd.OrdemCalculo,
            ordemExibicao: cmd.OrdemExibicao,
            prioridadeDesconto: cmd.PrioridadeDesconto,
            tetoMaximo: cmd.TetoMaximo,
            pisoMinimo: cmd.PisoMinimo,
            ativo: cmd.Ativo,
            dataInicioVigencia: cmd.DataInicioVigencia.HasValue ? new DateTime(cmd.DataInicioVigencia.Value.Year, cmd.DataInicioVigencia.Value.Month, cmd.DataInicioVigencia.Value.Day) : null,
            dataFimVigencia: cmd.DataFimVigencia.HasValue ? new DateTime(cmd.DataFimVigencia.Value.Year, cmd.DataFimVigencia.Value.Month, cmd.DataFimVigencia.Value.Day) : null,
            observacao: cmd.Observacao,
            grupoRubricaId: cmd.GrupoRubricaId);

        await _repo.AddAsync(rubrica, ct);

        return Result<RubricaDto>.Success(RubricaMapper.Map(rubrica));
    }

    private static RubricaDto MapRubrica(Rubrica r) => RubricaMapper.Map(r);
}

internal static class RubricaMapper
{
    public static RubricaDto Map(Rubrica r) => new()
    {
        Id = r.Id,
        EmpresaId = r.EmpresaId,
        GrupoRubricaId = r.GrupoRubricaId,
        Codigo = r.Codigo,
        Descricao = r.Descricao,
        DescricaoAbreviada = r.DescricaoAbreviada,
        Natureza = r.Natureza,
        TipoEsocial = r.TipoEsocial,
        EnviarEsocial = r.EnviarEsocial,
        IncideInss = r.IncideInss,
        IncideIrrf = r.IncideIrrf,
        IncideFgts = r.IncideFgts,
        IncideContribuicaoSindical = r.IncideContribuicaoSindical,
        IncideDecimoTerceiro = r.IncideDecimoTerceiro,
        IncideFerias = r.IncideFerias,
        IncideAvisoPrevio = r.IncideAvisoPrevio,
        IncideRescisao = r.IncideRescisao,
        IncideDissidio = r.IncideDissidio,
        IncideSalarioMaternidade = r.IncideSalarioMaternidade,
        IncideAuxilioDoenca = r.IncideAuxilioDoenca,
        IncideAdiantamento = r.IncideAdiantamento,
        TipoCalculo = r.TipoCalculo,
        FormulaCalculo = r.FormulaCalculo,
        ValorFixo = r.ValorFixo,
        Percentual = r.Percentual,
        RubricaBaseId = r.RubricaBaseId,
        OrdemCalculo = r.OrdemCalculo,
        OrdemExibicao = r.OrdemExibicao,
        PrioridadeDesconto = r.PrioridadeDesconto,
        TetoMaximo = r.TetoMaximo,
        PisoMinimo = r.PisoMinimo,
        Ativo = r.Ativo,
        DataInicioVigencia = r.DataInicioVigencia,
        DataFimVigencia = r.DataFimVigencia,
        Observacao = r.Observacao,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };
}

public class AtualizarRubricaHandler : IRequestHandler<AtualizarRubricaCommand, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    private readonly IMessageBus _messageBus;

    public AtualizarRubricaHandler(IRubricaRepository repo, IMessageBus messageBus)
    {
        _repo = repo;
        _messageBus = messageBus;
    }

    public async Task<Result<RubricaDto>> Handle(AtualizarRubricaCommand cmd, CancellationToken ct)
    {
        if (cmd.Id == Guid.Empty)
            return Result<RubricaDto>.Failure("VALIDACAO", "Id é obrigatório.");

        var rubrica = await _repo.GetByIdAsync(cmd.Id, ct);
        if (rubrica is null)
            return Result<RubricaDto>.Failure("NAO_ENCONTRADO", "Rubrica não encontrada.");

        rubrica.Atualizar(
            descricao: cmd.Descricao,
            natureza: cmd.Natureza,
            tipoEsocial: cmd.TipoEsocial,
            descricaoAbreviada: cmd.DescricaoAbreviada,
            enviarEsocial: cmd.EnviarEsocial,
            incideInss: cmd.IncideInss,
            incideIrrf: cmd.IncideIrrf,
            incideFgts: cmd.IncideFgts,
            incideContribuicaoSindical: cmd.IncideContribuicaoSindical,
            incideDecimoTerceiro: cmd.IncideDecimoTerceiro,
            incideFerias: cmd.IncideFerias,
            incideAvisoPrevio: cmd.IncideAvisoPrevio,
            incideRescisao: cmd.IncideRescisao,
            incideDissidio: cmd.IncideDissidio,
            incideSalarioMaternidade: cmd.IncideSalarioMaternidade,
            incideAuxilioDoenca: cmd.IncideAuxilioDoenca,
            incideAdiantamento: cmd.IncideAdiantamento,
            tipoCalculo: cmd.TipoCalculo,
            formulaCalculo: cmd.FormulaCalculo,
            valorFixo: cmd.ValorFixo,
            percentual: cmd.Percentual,
            rubricaBaseId: cmd.RubricaBaseId,
            ordemCalculo: cmd.OrdemCalculo,
            ordemExibicao: cmd.OrdemExibicao,
            prioridadeDesconto: cmd.PrioridadeDesconto,
            tetoMaximo: cmd.TetoMaximo,
            pisoMinimo: cmd.PisoMinimo,
            ativo: cmd.Ativo,
            dataInicioVigencia: cmd.DataInicioVigencia.HasValue ? new DateTime(cmd.DataInicioVigencia.Value.Year, cmd.DataInicioVigencia.Value.Month, cmd.DataInicioVigencia.Value.Day) : null,
            dataFimVigencia: cmd.DataFimVigencia.HasValue ? new DateTime(cmd.DataFimVigencia.Value.Year, cmd.DataFimVigencia.Value.Month, cmd.DataFimVigencia.Value.Day) : null,
            observacao: cmd.Observacao,
            grupoRubricaId: cmd.GrupoRubricaId);

        await _repo.UpdateAsync(rubrica, ct);

        await _messageBus.PublishAsync(new RubricaAlteradaEvent(
            rubrica.Id, rubrica.EmpresaId, rubrica.Codigo, rubrica.Natureza,
            new RubricaIncidencias(rubrica.IncideInss, rubrica.IncideIrrf, rubrica.IncideFgts,
                rubrica.IncideContribuicaoSindical, rubrica.IncideDecimoTerceiro,
                rubrica.IncideFerias, rubrica.IncideAvisoPrevio)),
            "folha360.cadastros", "RubricaAlterada", ct);

        return Result<RubricaDto>.Success(RubricaMapper.Map(rubrica));
    }
}

public class ExcluirRubricaHandler : IRequestHandler<ExcluirRubricaCommand, Result<bool>>
{
    private readonly IRubricaRepository _repo;
    public ExcluirRubricaHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(ExcluirRubricaCommand cmd, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(cmd.Id, ct);
        return Result<bool>.Success(true);
    }
}

public class ObterRubricaHandler : IRequestHandler<ObterRubricaQuery, Result<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    public ObterRubricaHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<Result<RubricaDto>> Handle(ObterRubricaQuery query, CancellationToken ct)
    {
        var r = await _repo.GetByIdAsync(query.Id, ct);
        if (r is null)
            return Result<RubricaDto>.Failure("NAO_ENCONTRADO", "Rubrica não encontrada.");

        return Result<RubricaDto>.Success(new RubricaDto
        {
            Id = r.Id,
            EmpresaId = r.EmpresaId,
            Codigo = r.Codigo,
            Descricao = r.Descricao,
            Natureza = r.Natureza,
            TipoEsocial = r.TipoEsocial,
            IncideInss = r.IncideInss,
            IncideIrrf = r.IncideIrrf,
            IncideFgts = r.IncideFgts,
            IncideContribuicaoSindical = r.IncideContribuicaoSindical,
            IncideDecimoTerceiro = r.IncideDecimoTerceiro,
            IncideFerias = r.IncideFerias,
            IncideAvisoPrevio = r.IncideAvisoPrevio,
            FormulaCalculo = r.FormulaCalculo,
            OrdemExibicao = r.OrdemExibicao,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        });
    }
}

public class ListarRubricasHandler : IRequestHandler<ListarRubricasQuery, PaginatedResult<RubricaDto>>
{
    private readonly IRubricaRepository _repo;
    public ListarRubricasHandler(IRubricaRepository repo) => _repo = repo;

    public async Task<PaginatedResult<RubricaDto>> Handle(ListarRubricasQuery query, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            query.Page, query.PageSize, query.OrderBy,
            query.EmpresaId, query.Natureza, query.TipoEsocial, ct);

        var dtos = items.Select(r => new RubricaDto
        {
            Id = r.Id,
            EmpresaId = r.EmpresaId,
            Codigo = r.Codigo,
            Descricao = r.Descricao,
            Natureza = r.Natureza,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
        });

        return PaginatedResult<RubricaDto>.Success(dtos, query.Page, query.PageSize, total);
    }
}
