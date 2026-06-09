using Folha360.Application;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Domain.Abstractions;
using Folha360.Fiscais.Application.Commands;
using Folha360.Fiscais.Application.DTOs;
using Folha360.Fiscais.Domain;
using Folha360.Fiscais.Domain.Abstractions;
using Folha360.Fiscais.Domain.Entities;
using Folha360.Fiscais.Domain.Events;
using Folha360.Processamento.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Folha360.Fiscais.Application.Handlers;

public class ApurarObrigacoesHandler : IRequestHandler<ApurarObrigacoesCommand, Result<ResumoApuracaoDto>>
{
    private readonly IApuracaoFiscalRepository _apuracaoRepo;
    private readonly IRegraFiscalRepository _regraRepo;
    private readonly IRegraFiscalFactory _regraFactory;
    private readonly IEmpresaRepository _empresaRepo;
    private readonly IProcessamentoRepository _processamentoRepo;
    private readonly IItemFolhaRepository _itemFolhaRepo;
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
        IEmpresaRepository empresaRepo,
        IProcessamentoRepository processamentoRepo,
        IItemFolhaRepository itemFolhaRepo,
        IMessageBus messageBus,
        ILogger<ApurarObrigacoesHandler> logger)
    {
        _apuracaoRepo = apuracaoRepo;
        _regraRepo = regraRepo;
        _regraFactory = regraFactory;
        _empresaRepo = empresaRepo;
        _processamentoRepo = processamentoRepo;
        _itemFolhaRepo = itemFolhaRepo;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result<ResumoApuracaoDto>> Handle(ApurarObrigacoesCommand request, CancellationToken ct)
    {
        var periodo = DateOnly.ParseExact(request.Periodo + "-01", "yyyy-MM-dd");

        // Idempotência
        foreach (var tributo in Tributos)
        {
            if (await _apuracaoRepo.ExistsAsync(request.EmpresaId, periodo, tributo, request.ProcessamentoId, ct))
            {
                return Result<ResumoApuracaoDto>.Failure("DUPLICATE", "Apuração fiscal já existe para este processamento.");
            }
        }

        // Buscar dados da empresa (regime tributário, município) — F02
        var empresa = await _empresaRepo.GetByIdAsync(request.EmpresaId, ct);
        var regimeTributario = empresa?.RegimeTributario ?? "Lucro Presumido";
        var municipio = empresa?.EnderecoMunicipio;

        // Buscar dados do processamento da folha — F04
        var processamento = await _processamentoRepo.GetByIdAsync(request.ProcessamentoId, ct);
        if (processamento == null)
        {
            return Result<ResumoApuracaoDto>.Failure("NOT_FOUND", $"Processamento {request.ProcessamentoId} não encontrado.");
        }

        // Buscar itens da folha para obter bases de cálculo e valores
        var itensFolha = await _itemFolhaRepo.GetByProcessamentoAsync(request.ProcessamentoId, ct);
        var itensList = itensFolha.ToList();
        var funcionariosIds = itensList.Select(i => i.FuncionarioId).Distinct().ToList();
        var baseCalculoTotal = itensList.Sum(i => i.BaseCalculo);
        var valorTotalFolha = itensList.Sum(i => i.Valor);

        var totais = new Dictionary<string, decimal>();
        var totaisPorTributo = new Dictionary<Tributo, decimal>();

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
                    regimeTributario,
                    municipio,
                    funcionariosIds,
                    baseCalculoTotal,
                    valorTotalFolha);

                var parametros = new RegraFiscalParametros(tributo, regra.Parametros, regra.CodigoReceita);
                var result = service.Calcular(contexto, parametros);

                var apuracao = new ApuracaoFiscal(request.EmpresaId, periodo, request.ProcessamentoId, tributo);
                apuracao.Concluir(result.BaseCalculo, result.Aliquota, result.ValorDevido, result.DataVencimento, regra.Id);
                await _apuracaoRepo.AddAsync(apuracao, ct);

                totais[tributo.ToString()] = result.ValorDevido;
                totaisPorTributo[tributo] = result.ValorDevido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao apurar tributo {Tributo} para empresa {EmpresaId}", tributo, request.EmpresaId);
            }
        }

        // Publicar eventos com totais reais
        var ocorridoEm = DateTime.UtcNow;
        await _messageBus.PublishAsync(
            new ObrigacoesApuradasEvent(request.EmpresaId, request.Periodo, request.ProcessamentoId, totaisPorTributo, ocorridoEm),
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
