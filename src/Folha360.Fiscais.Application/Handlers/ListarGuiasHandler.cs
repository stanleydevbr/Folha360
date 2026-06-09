using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ListarGuiasHandler : IRequestHandler<ListarGuiasQuery, Result<List<GuiaRecolhimentoDto>>>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public ListarGuiasHandler(IGuiaRecolhimentoRepository guiaRepo)
    {
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<List<GuiaRecolhimentoDto>>> Handle(ListarGuiasQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var guias = await _guiaRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var dtos = guias.Select(g => new GuiaRecolhimentoDto(
            g.Id,
            g.TipoGuia.ToString(),
            g.Tributo.ToString(),
            g.Valor,
            g.DataVencimento.ToString("yyyy-MM-dd"),
            g.Status.ToString(),
            g.MinioKey != null ? $"/api/fiscais/guias/{g.EmpresaId}/{request.Periodo}/{g.Tributo}" : null)).ToList();

        return Result<List<GuiaRecolhimentoDto>>.Success(dtos);
    }
}
