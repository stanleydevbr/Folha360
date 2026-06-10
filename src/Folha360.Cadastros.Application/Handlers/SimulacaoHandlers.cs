using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Services;
using MediatR;

namespace Folha360.Cadastros.Application.Handlers;

public class SimularRubricaHandler : IRequestHandler<SimularRubricaCommand, Result<SimulacaoResultadoDto>>
{
    private readonly IRubricaRepository _rubricaRepo;
    private readonly MotorCalculo _motorCalculo;

    public SimularRubricaHandler(IRubricaRepository rubricaRepo, MotorCalculo motorCalculo)
    {
        _rubricaRepo = rubricaRepo;
        _motorCalculo = motorCalculo;
    }

    public async Task<Result<SimulacaoResultadoDto>> Handle(SimularRubricaCommand cmd, CancellationToken ct)
    {
        var rubricas = new List<Rubrica>();
        foreach (var id in cmd.RubricasIds)
        {
            var rubrica = await _rubricaRepo.GetByIdAsync(id, ct);
            if (rubrica is not null)
            {
                rubricas.Add(rubrica);
            }
        }

        var contexto = new Dictionary<string, object>
        {
            ["salario_base"] = cmd.SalarioBase,
            ["tipo_contrato"] = cmd.TipoContrato ?? "CLT",
            ["quantidade_horas"] = cmd.QuantidadeHoras ?? 0m,
            ["quantidade_dias"] = cmd.QuantidadeDias ?? 0m,
            ["valor_hora"] = cmd.SalarioBase / 220m,
            ["valor_dia"] = cmd.SalarioBase / 30m
        };

        var resultado = _motorCalculo.Calcular(rubricas, contexto);

        return Result<SimulacaoResultadoDto>.Success(new SimulacaoResultadoDto
        {
            ValoresPorRubrica = resultado.ValoresPorRubrica,
            TotalVencimentos = resultado.TotalVencimentos,
            TotalDescontos = resultado.TotalDescontos,
            Liquido = resultado.Liquido,
            BaseInss = resultado.BaseInss,
            BaseIrrf = resultado.BaseIrrf,
            BaseFgts = resultado.BaseFgts,
            Erros = resultado.Erros
        });
    }
}

public class VerificarConformidadeHandler : IRequestHandler<VerificarConformidadeQuery, Result<List<ConformidadeRubricaDto>>>
{
    private readonly IRubricaRepository _rubricaRepo;

    public VerificarConformidadeHandler(IRubricaRepository rubricaRepo)
    {
        _rubricaRepo = rubricaRepo;
    }

    public async Task<Result<List<ConformidadeRubricaDto>>> Handle(VerificarConformidadeQuery query, CancellationToken ct)
    {
        var rubricas = await _rubricaRepo.GetAllByEmpresaAsync(query.EmpresaId, ct);
        var problemas = new List<ConformidadeRubricaDto>();

        foreach (var r in rubricas.Where(r => r.EnviarEsocial && r.Ativo))
        {
            if (string.IsNullOrWhiteSpace(r.TipoEsocial))
            {
                problemas.Add(new ConformidadeRubricaDto
                {
                    RubricaId = r.Id,
                    Codigo = r.Codigo,
                    Descricao = r.Descricao,
                    TipoEsocial = r.TipoEsocial,
                    Problema = "TIPO_ESOCIAL_AUSENTE"
                });
            }
        }

        return Result<List<ConformidadeRubricaDto>>.Success(problemas);
    }
}
