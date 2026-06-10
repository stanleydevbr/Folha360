using Folha360.Application;
using Folha360.Processamento.Application.DTOs;
using MediatR;

namespace Folha360.Processamento.Application.Commands;

public sealed record IniciarProcessamentoCommand : IRequest<Result<ProcessamentoResponse>>
{
    public Guid EmpresaId { get; init; }
    public string Periodo { get; init; } = string.Empty;
    public string TipoCalculo { get; init; } = string.Empty;
}

public sealed record ReprocessarFolhaCommand : IRequest<Result<ProcessamentoResponse>>
{
    public Guid ProcessamentoId { get; init; }
}

public sealed record CancelarProcessamentoCommand : IRequest<Result<bool>>
{
    public Guid ProcessamentoId { get; init; }
}
