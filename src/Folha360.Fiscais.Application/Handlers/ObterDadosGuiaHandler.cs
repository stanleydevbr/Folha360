using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterDadosGuiaHandler : IRequestHandler<ObterDadosGuiaQuery, Result<GuiaRecolhimentoDto>>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public ObterDadosGuiaHandler(IGuiaRecolhimentoRepository guiaRepo)
    {
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<GuiaRecolhimentoDto>> Handle(ObterDadosGuiaQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var guias = await _guiaRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var guia = guias.FirstOrDefault(g => g.Tributo.ToString() == request.Tributo);
        if (guia == null)
        {
            return Result<GuiaRecolhimentoDto>.Failure("NOT_FOUND", "Guia não encontrada.");
        }

        return Result<GuiaRecolhimentoDto>.Success(new GuiaRecolhimentoDto(
            guia.Id,
            guia.TipoGuia.ToString(),
            guia.Tributo.ToString(),
            guia.Valor,
            guia.DataVencimento.ToString("yyyy-MM-dd"),
            guia.Status.ToString(),
            null));
    }
}
