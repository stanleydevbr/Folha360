using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Application.Queries;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterGuiaHandler : IRequestHandler<ObterGuiaQuery, Result<Stream>>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public ObterGuiaHandler(IGuiaRecolhimentoRepository guiaRepo)
    {
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<Stream>> Handle(ObterGuiaQuery request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var guias = await _guiaRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var guia = guias.FirstOrDefault(g => g.Tributo.ToString() == request.Tributo);
        if (guia == null || guia.MinioKey == null)
        {
            return Result<Stream>.Failure("NOT_FOUND", "Guia não encontrada.");
        }

        // Retorna stream vazio como placeholder (PDF será implementado nos serviços de infra)
        var stream = new MemoryStream();
        return Result<Stream>.Success(stream);
    }
}
