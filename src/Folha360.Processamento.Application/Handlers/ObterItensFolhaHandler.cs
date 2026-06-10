using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ObterItensFolhaHandler : IRequestHandler<ObterItensFolhaQuery, Result<List<ItemFolhaResponse>>>
{
    private readonly IItemFolhaRepository _repo;

    public ObterItensFolhaHandler(IItemFolhaRepository repo) => _repo = repo;

    public async Task<Result<List<ItemFolhaResponse>>> Handle(
        ObterItensFolhaQuery query, CancellationToken ct)
    {
        var itens = query.FuncionarioId.HasValue
            ? await _repo.GetByFuncionarioAsync(query.ProcessamentoId, query.FuncionarioId.Value, ct)
            : await _repo.GetByProcessamentoAsync(query.ProcessamentoId, ct);

        var responses = itens.Select(i => new ItemFolhaResponse(
            i.Id, i.RubricaId, string.Empty, string.Empty,
            i.Fase.ToString(), i.BaseCalculo, i.Valor,
            i.FormulaAplicada, i.Ordem)).ToList();

        return Result<List<ItemFolhaResponse>>.Success(responses);
    }
}
