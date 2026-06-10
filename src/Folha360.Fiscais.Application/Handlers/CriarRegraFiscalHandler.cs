using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class CriarRegraFiscalHandler : IRequestHandler<CriarRegraFiscalCommand, Result<RegraFiscalDto>>
{
    private readonly IRegraFiscalRepository _regraRepo;

    public CriarRegraFiscalHandler(IRegraFiscalRepository regraRepo)
    {
        _regraRepo = regraRepo;
    }

    public async Task<Result<RegraFiscalDto>> Handle(CriarRegraFiscalCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<Tributo>(request.Tributo, out var tributo))
        {
            var erros = new List<Error>();
            erros.Add(new Error("INVALID_TRIBUTO", $"Tributo inválido: {request.Tributo}"));
            return Result<RegraFiscalDto>.Failure(erros);
        }

        if (await _regraRepo.ExistsAsync(tributo, request.Versao, ct))
        {
            var erros = new List<Error>();
            erros.Add(new Error("DUPLICATE", $"Já existe uma regra fiscal para {request.Tributo} versão {request.Versao}."));
            return Result<RegraFiscalDto>.Failure(erros);
        }

        var regra = new RegraFiscal(
            tributo,
            request.Versao,
            DateOnly.Parse(request.VigenciaInicio),
            request.VigenciaFim != null ? DateOnly.Parse(request.VigenciaFim) : null,
            request.Parametros,
            request.CodigoReceita);

        await _regraRepo.AddAsync(regra, ct);

        return Result<RegraFiscalDto>.Success(new RegraFiscalDto(
            regra.Id,
            regra.Tributo.ToString(),
            regra.Versao,
            regra.VigenciaInicio.ToString("yyyy-MM-dd"),
            regra.VigenciaFim?.ToString("yyyy-MM-dd"),
            regra.CodigoReceita,
            regra.Ativo));
    }
}
