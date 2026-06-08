using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using MediatR;

namespace Folha360.Processamento.Application.Queries;

public sealed record ObterHoleriteQuery(Guid ProcessamentoId, Guid FuncionarioId)
    : IRequest<Result<byte[]>>;

public sealed record ListarHoleritesQuery(Guid ProcessamentoId)
    : IRequest<Result<List<HoleriteResponse>>>;
