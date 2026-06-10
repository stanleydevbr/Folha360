using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using Folha360.Processamento.Application.Queries;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;

namespace Folha360.Processamento.Application.Handlers;

public class ObterProcessamentoHandler : IRequestHandler<ObterProcessamentoQuery, Result<ProcessamentoResponse>>
{
    private readonly IProcessamentoRepository _repo;

    public ObterProcessamentoHandler(IProcessamentoRepository repo) => _repo = repo;

    public async Task<Result<ProcessamentoResponse>> Handle(
        ObterProcessamentoQuery query, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(query.Id, ct);
        if (p is null)
            return Result<ProcessamentoResponse>.Failure("NAO_ENCONTRADO", "Processamento não encontrado.");

        return Result<ProcessamentoResponse>.Success(new ProcessamentoResponse(
            p.Id,
            p.EmpresaId,
            $"{p.Periodo:yyyy-MM}",
            p.TipoCalculo.ToString(),
            p.Status.ToString(),
            p.Versao,
            p.TotalFuncionarios,
            p.FuncionariosProcessados,
            p.FuncionariosComErro,
            p.TotalVencimentos,
            p.TotalDescontos,
            p.TotalLiquido,
            p.DataInicio,
            p.DataFim,
            p.Erro));
    }
}
