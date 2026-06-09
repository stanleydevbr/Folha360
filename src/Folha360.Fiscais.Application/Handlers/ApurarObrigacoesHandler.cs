using Folha360.Application;
using Folha360.Domain.Abstractions;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Handlers;

public class ApurarObrigacoesHandler : IRequestHandler<ApurarObrigacoesCommand, Result<ResumoApuracaoDto>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;
    private readonly IRegraFiscalRepository _regraRepo;
    private readonly IRegraFiscalFactory _regraFactory;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ApurarObrigacoesHandler> _logger;

    private static readonly Tributo[] Tributos = new[]
    {
        Tributo.IRRF, Tributo.INSS, Tributo.FGTS,
        Tributo.ContribuicaoSindical, Tributo.PIS, Tributo.COFINS,
        Tributo.CSLL, Tributo.ISS,
    };

    public ApurarObrigacoesHandler(
        IApuracaoFiscalRepository apuracaoRepo,
        IRegraFiscalRepository regraRepo,
        IRegraFiscalFactory regraFactory,
        IMessageBus messageBus,
        ILogger<ApurarObrigacoesHandler> logger)
    {
        _apuracaoRepo = apuracaoRepo;
        _regraRepo = regraRepo;
        _regraFactory = regraFactory;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<ResumoApuracaoDto>> Handle(ApurarObrigacoesCommand request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");

        // Idempotência: verificar se já existe apuração para este processamento
        foreach (var tributo in Tributos)
        {
            if (await _apuracaoRepo.ExistsAsync(request.EmpresaId, periodo, tributo, request.ProcessamentoId, ct))
            {
                return Result<ResumoApuracaoDto>.Failure("DUPLICATE", "Apuração fiscal já existe para este processamento.");
            }
        }

        var totais = new Dictionary<string, decimal>();
        var apuracoes = new List<ApuracaoFiscal>();

        foreach (var tributo in Tributos)
        {
            try
            {
                var regra = await _regraRepo.GetVigenteAsync(tributo, periodo, ct);
                if (regra == null)
                {
                    _logger.LogWarning("Regra fiscal não encontrada para {Tributo} no período {Periodo}", tributo, request.Periodo);
                    continue;
                }

                var service = _regraFactory.Resolver(tributo);
                var contexto = new ApuracaoContext(
                    request.EmpresaId,
                    periodo,
                    request.ProcessamentoId,
                    "Lucro Presumido", // TODO: buscar do cadastro da empresa
                    null,
                    new List<Guid>(),
                    0,
                    0);

                var parametros = new RegraFiscalParametros(tributo, regra.Parametros, regra.CodigoReceita);
                var result = service.Calcular(contexto, parametros);

                var apuracao = new ApuracaoFiscal(request.EmpresaId, periodo, request.ProcessamentoId, tributo);
                apuracao.Concluir(result.BaseCalculo, result.Aliquota, result.ValorDevido, result.DataVencimento, regra.Id);
                await _apuracaoRepo.AddAsync(apuracao, ct);
                apuracoes.Add(apuracao);

                totais[tributo.ToString()] = result.ValorDevido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao apurar tributo {Tributo} para empresa {EmpresaId}", tributo, request.EmpresaId);
            }
        }

        // Publicar eventos
        var ocorridoEm = DateTime.UtcNow;
        await _messageBus.PublishAsync(
            new ObrigacoesApuradasEvent(request.EmpresaId, request.Periodo, request.ProcessamentoId, new Dictionary<Tributo, decimal>(), ocorridoEm),
            "folha360.fiscais",
            "obrigacoes.apuradas",
            ct);

        return Result<ResumoApuracaoDto>.Success(new ResumoApuracaoDto(
            request.EmpresaId,
            request.Periodo,
            request.ProcessamentoId,
            "Concluido",
            totais,
            new List<GuiaRecolhimentoDto>(),
            ocorridoEm));
    }
}
