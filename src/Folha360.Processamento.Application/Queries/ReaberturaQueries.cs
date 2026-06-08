using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using MediatR;

namespace Folha360.Processamento.Application.Queries;

public sealed record ObterHistoricoProcessamentoQuery(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<List<HistoricoProcessamentoResponse>>>;

public sealed record ObterReaberturaStatusQuery(Guid ProcessamentoId)
    : IRequest<Result<ReaberturaStatusResponse>>;

public sealed record ObterFechamentoStatusQuery(Guid EmpresaId, string Periodo)
    : IRequest<Result<ReaberturaStatusResponse>>;
