using Folha360.Application;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ExportarLancamentosHandler : IRequestHandler<ExportarLancamentosCommand, Result<List<ExportacaoDto>>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;
    private readonly ILancamentoContabilRepository _lancamentoRepo;

    public ExportarLancamentosHandler(
        IApuracaoFiscalRepository apuracaoRepo,
        ILancamentoContabilRepository lancamentoRepo)
    {
        _apuracaoRepo = apuracaoRepo;
        _lancamentoRepo = lancamentoRepo;
    }

    public async Task<Result<List<ExportacaoDto>>> Handle(ExportarLancamentosCommand request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");
        var apuracoes = await _apuracaoRepo.GetByEmpresaPeriodoAsync(request.EmpresaId, periodo, ct);

        var dtos = new List<ExportacaoDto>();

        foreach (var apuracao in apuracoes)
        {
            var lancamento = new LancamentoContabil(
                request.EmpresaId,
                periodo,
                apuracao.Id,
                periodo,
                "DESPESA_FOLHA",
                "CAIXA_BANCOS",
                $"Apuração {apuracao.Tributo} - {request.Periodo}",
                apuracao.ValorDevido,
                apuracao.Tributo,
                FormatoExportacao.CSV);

            await _lancamentoRepo.AddAsync(lancamento, ct);

            dtos.Add(new ExportacaoDto(
                lancamento.Id,
                lancamento.Formato.ToString(),
                null,
                lancamento.CreatedAt));
        }

        return Result<List<ExportacaoDto>>.Success(dtos);
    }
}
