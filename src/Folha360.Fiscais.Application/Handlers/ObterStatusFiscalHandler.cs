using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterStatusFiscalHandler : IRequestHandler<ObterStatusFiscalQuery, Result<StatusFiscalDto>>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public ObterStatusFiscalHandler(IGuiaRecolhimentoRepository guiaRepo)
    {
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<StatusFiscalDto>> Handle(ObterStatusFiscalQuery request, CancellationToken ct)
    {
        var pendentes = await _guiaRepo.GetPendentesAsync(request.EmpresaId, ct);
        var vencidas = await _guiaRepo.GetVencidasAsync(request.EmpresaId, ct);

        return Result<StatusFiscalDto>.Success(new StatusFiscalDto(
            null,
            pendentes.Count,
            vencidas.Count,
            new List<DateTime>()));
    }
}
