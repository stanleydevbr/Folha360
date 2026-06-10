using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Processamento.Application.Services;
using Folha360.Processamento.Domain;
using Folha360.Processamento.Domain.Abstractions;
using Folha360.Processamento.Domain.Entities;
using Folha360.Processamento.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Folha360.Processamento.Application.Consumers;

public class ProcessarFolhaConsumer : IConsumer<Folha360.Processamento.Application.Handlers.ProcessarFolhaInternalCommand>
{
    private readonly IProcessamentoRepository _processamentoRepo;
    private readonly IItemFolhaRepository _itemFolhaRepo;
    private readonly IFuncionarioRepository _funcionarioRepo;
    private readonly IRubricaRepository _rubricaRepo;
    private readonly IMotorCalculo _motorCalculo;
    private readonly IPdfGeradorService _pdfGerador;
    private readonly IRedisCacheService _cache;
    private readonly ILogger<ProcessarFolhaConsumer> _logger;

    public ProcessarFolhaConsumer(
        IProcessamentoRepository processamentoRepo,
        IItemFolhaRepository itemFolhaRepo,
        IFuncionarioRepository funcionarioRepo,
        IRubricaRepository rubricaRepo,
        IMotorCalculo motorCalculo,
        IPdfGeradorService pdfGerador,
        IRedisCacheService cache,
        ILogger<ProcessarFolhaConsumer> logger)
    {
        _processamentoRepo = processamentoRepo;
        _itemFolhaRepo = itemFolhaRepo;
        _funcionarioRepo = funcionarioRepo;
        _rubricaRepo = rubricaRepo;
        _motorCalculo = motorCalculo;
        _pdfGerador = pdfGerador;
        _cache = cache;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Folha360.Processamento.Application.Handlers.ProcessarFolhaInternalCommand> context)
    {
        var cmd = context.Message;
        var processamento = await _processamentoRepo.GetByIdAsync(cmd.ProcessamentoId, context.CancellationToken);

        if (processamento is null)
        {
            _logger.LogError("Processamento {Id} não encontrado", cmd.ProcessamentoId);
            return;
        }

        try
        {
            // Iniciar processamento
            var funcionarios = await _funcionarioRepo.GetAllAsync(context.CancellationToken);
            var funcionariosList = funcionarios.Where(f => f.EmpresaId == processamento.EmpresaId && f.Status == "Ativo").ToList();

            processamento.Iniciar(funcionariosList.Count);
            await _processamentoRepo.UpdateAsync(processamento, context.CancellationToken);

            // Buscar rubricas (tentar cache primeiro)
            var cacheKey = $"cache:rubricas:{processamento.EmpresaId}";
            var rubricas = await _cache.GetAsync<List<Folha360.Cadastros.Domain.Entities.Rubrica>>(cacheKey, context.CancellationToken);

            if (rubricas is null)
            {
                var todasRubricas = await _rubricaRepo.GetAllAsync(context.CancellationToken);
                rubricas = todasRubricas.Where(r => r.EmpresaId == processamento.EmpresaId && r.Ativo).ToList();
                await _cache.SetAsync(cacheKey, rubricas, TimeSpan.FromHours(1), context.CancellationToken);
            }

            var processados = 0;
            var comErro = 0;
            decimal totalVencimentos = 0;
            decimal totalDescontos = 0;
            decimal totalLiquido = 0;
            decimal totalFgts = 0;

            foreach (var funcionario in funcionariosList)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var contextoFuncionario = new Dictionary<string, object>
                    {
                        ["SALARIO_BASE"] = funcionario.SalarioBase,
                        ["FUNCIONARIO_ID"] = funcionario.Id,
                        ["CARGO_ID"] = funcionario.CargoId,
                    };

                    var resultado = _motorCalculo.Processar(
                        funcionario.Id,
                        rubricas,
                        contextoFuncionario,
                        processamento.TipoCalculo,
                        processamento.Periodo,
                        context.CancellationToken);

                    // Persistir itens da folha
                    var itens = new List<ItemFolha>();
                    int ordem = 1;
                    foreach (var (rubricaId, valor) in resultado.ValoresPorRubrica)
                    {
                        var rubrica = rubricas.FirstOrDefault(r => r.Id == rubricaId);
                        var fase = rubrica is not null
                            ? rubrica.OrdemCalculo switch
                            {
                                < 100 => FaseProcessamento.Vencimentos,
                                < 200 => FaseProcessamento.Bases,
                                < 300 => FaseProcessamento.Descontos,
                                _ => FaseProcessamento.Totais,
                            }
                            : FaseProcessamento.Vencimentos;

                        itens.Add(new ItemFolha(
                            processamento.Id,
                            funcionario.Id,
                            rubricaId,
                            fase,
                            0,
                            valor,
                            rubrica?.FormulaCalculo,
                            ordem++));
                    }

                    await _itemFolhaRepo.AddBatchAsync(itens, context.CancellationToken);

                    totalVencimentos += resultado.TotalVencimentos;
                    totalDescontos += resultado.TotalDescontos;
                    totalLiquido += resultado.Liquido;
                    totalFgts += resultado.BaseFgts * 0.08m;
                }
                catch (Exception ex)
                {
                    comErro++;
                    _logger.LogWarning(ex, "Erro ao processar funcionário {FuncionarioId}", funcionario.Id);
                }

                processados++;
            }

            processamento.Concluir(totalVencimentos, totalDescontos, totalLiquido, totalFgts);
            processamento.AtualizarProgresso(processados, comErro);
            await _processamentoRepo.UpdateAsync(processamento, context.CancellationToken);

            _logger.LogInformation(
                "Processamento concluído: {ProcessamentoId}, {Processados} processados, {Erros} erros",
                processamento.Id, processados, comErro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha no processamento {ProcessamentoId}", cmd.ProcessamentoId);
            processamento.Falhar(ex.Message);
            await _processamentoRepo.UpdateAsync(processamento, context.CancellationToken);
        }
    }
}
