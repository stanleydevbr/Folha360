using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Domain.Abstractions;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class RegistrarPagamentoGuiaHandler : IRequestHandler<RegistrarPagamentoGuiaCommand, Result<bool>>
{
    private readonly IGuiaRecolhimentoRepository _guiaRepo;

    public RegistrarPagamentoGuiaHandler(IGuiaRecolhimentoRepository guiaRepo)
    {
        _guiaRepo = guiaRepo;
    }

    public async Task<Result<bool>> Handle(RegistrarPagamentoGuiaCommand request, CancellationToken ct)
    {
        var guia = await _guiaRepo.GetByIdAsync(request.GuiaId, ct);
        if (guia == null)
        {
            return Result<bool>.Failure("NOT_FOUND", "Guia não encontrada.");
        }

        guia.RegistrarPagamento(request.ValorPago, request.DataPagamento);
        await _guiaRepo.UpdateAsync(guia, ct);

        return Result<bool>.Success(true);
    }
}
