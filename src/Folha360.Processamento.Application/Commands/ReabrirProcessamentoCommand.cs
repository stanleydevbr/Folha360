using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using MediatR;

namespace Folha360.Processamento.Application.Commands;

public sealed record ReabrirProcessamentoCommand : IRequest<Result<ReaberturaStatusResponse>>
{
    public Guid ProcessamentoId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public Guid Autor { get; init; }
}
