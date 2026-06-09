using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ListarExportacoesHandler : IRequestHandler<ListarExportacoesQuery, Result<List<ExportacaoDto>>>
{
    private readonly ILancamentoContabilRepository _lancamentoRepo;

    public ListarExportacoesHandler(ILancamentoContabilRepository lancamentoRepo)
    {
        _lancamentoRepo = lancamentoRepo;
    }

    public async Task<Result<List<ExportacaoDto>>> Handle(ListarExportacoesQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var lancamentos = await _lancamentoRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var dtos = lancamentos.Select(l => new ExportacaoDto(
            l.Id,
            l.Formato.ToString(),
            l.MinioKey != null ? $"/api/fiscais/exportacoes/{l.EmpresaId}/{request.Periodo}" : null,
            l.CreatedAt)).ToList();

        return Result<List<ExportacaoDto>>.Success(dtos);
    }
}
