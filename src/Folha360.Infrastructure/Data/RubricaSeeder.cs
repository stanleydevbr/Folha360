using Folha360.Cadastros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Data;

public static class RubricaSeeder
{
    public static async Task SeedAsync(Folha360DbContext context, ILogger logger)
    {
        if (await context.Set<Rubrica>().AnyAsync())
        {
            logger.LogInformation("Rubricas already seeded. Skipping...");
            return;
        }

        logger.LogInformation("Seeding rubricas padrao (Tabela 03 e-Social)...");

        var empresaId = Guid.NewGuid(); // Será substituído pelo tenant real

        // Grupos de Rubrica
        var grupos = new List<GrupoRubrica>
        {
            new(empresaId, "VENC_FIXOS", "Vencimentos Fixos", "Vencimento", 1),
            new(empresaId, "VENC_VARIAVEIS", "Vencimentos Variáveis", "Vencimento", 2),
            new(empresaId, "DESC_LEGAIS", "Descontos Legais", "Desconto", 3),
            new(empresaId, "DESC_VOLUNT", "Descontos Voluntários", "Desconto", 4),
            new(empresaId, "BENEFICIOS", "Benefícios", "Beneficio", 5),
            new(empresaId, "PROVISOES", "Provisões", "Provisao", 6),
            new(empresaId, "BASES", "Bases de Cálculo", "Base", 7),
            new(empresaId, "TOTAIS", "Totais", "Informativo", 8)
        };
        context.Set<GrupoRubrica>().AddRange(grupos);

        // Rubricas padrão
        var rubricas = new List<Rubrica>
        {
            // Vencimentos Fixos
            new(empresaId, "SAL-BASE", "Salário Base Mensal", "Vencimento",
                tipoCalculo: "VALOR_FIXO", ordemCalculo: 10, ordemExibicao: 1, grupoRubricaId: grupos[0].Id),

            // Vencimentos Variáveis
            new(empresaId, "HORA-EXTRA-50", "Horas Extras 50%", "Vencimento",
                tipoCalculo: "HORA", incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 20, ordemExibicao: 2, grupoRubricaId: grupos[1].Id),

            new(empresaId, "HORA-EXTRA-100", "Horas Extras 100%", "Vencimento",
                tipoCalculo: "HORA", incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 21, ordemExibicao: 3, grupoRubricaId: grupos[1].Id),

            new(empresaId, "ADIC-NOTURNO", "Adicional Noturno 20%", "Vencimento",
                tipoCalculo: "PERCENTUAL", percentual: 20, incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 22, ordemExibicao: 4, grupoRubricaId: grupos[1].Id),

            new(empresaId, "ADIC-PERIC", "Adicional Periculosidade 30%", "Vencimento",
                tipoCalculo: "PERCENTUAL", percentual: 30, incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 23, ordemExibicao: 5, grupoRubricaId: grupos[1].Id),

            new(empresaId, "ADIC-INSAL", "Adicional Insalubridade", "Vencimento",
                tipoCalculo: "PERCENTUAL", percentual: 20, incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 24, ordemExibicao: 6, grupoRubricaId: grupos[1].Id),

            new(empresaId, "COMISSAO", "Comissão sobre Vendas", "Vencimento",
                tipoCalculo: "PERCENTUAL", incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 25, ordemExibicao: 7, grupoRubricaId: grupos[1].Id),

            new(empresaId, "DSR", "Descanso Semanal Remunerado", "Vencimento",
                tipoCalculo: "FORMULA", formulaCalculo: "{RUBRICA_HORA_EXTRA} / {DIAS_UTEIS} * {DIAS_DSR}",
                incideInss: true, incideIrrf: true, incideFgts: true,
                incideDecimoTerceiro: true, incideFerias: true, incideAvisoPrevio: true, incideRescisao: true,
                ordemCalculo: 26, ordemExibicao: 8, grupoRubricaId: grupos[1].Id),

            // Bases de Cálculo
            new(empresaId, "BASE-INSS", "Base de Cálculo INSS", "Base",
                tipoCalculo: "COMPOSICAO", ordemCalculo: 100, ordemExibicao: 50, grupoRubricaId: grupos[6].Id),

            new(empresaId, "BASE-FGTS", "Base de Cálculo FGTS", "Base",
                tipoCalculo: "COMPOSICAO", ordemCalculo: 101, ordemExibicao: 51, grupoRubricaId: grupos[6].Id),

            new(empresaId, "BASE-IRRF", "Base de Cálculo IRRF", "Base",
                tipoCalculo: "FORMULA", formulaCalculo: "{RUBRICA_BASE_INSS} - {RUBRICA_INSS} - {RUBRICA_PENSAO}",
                ordemCalculo: 102, ordemExibicao: 52, grupoRubricaId: grupos[6].Id),

            // Descontos Legais
            new(empresaId, "INSS", "INSS — Previdência Social", "Desconto",
                tipoCalculo: "TABELA_PROGRESSIVA", incideInss: false, incideIrrf: true,
                prioridadeDesconto: 1, ordemCalculo: 200, ordemExibicao: 100, grupoRubricaId: grupos[2].Id),

            new(empresaId, "IRRF", "IRRF — Imposto de Renda", "Desconto",
                tipoCalculo: "TABELA_PROGRESSIVA", incideIrrf: false,
                prioridadeDesconto: 2, ordemCalculo: 201, ordemExibicao: 101, grupoRubricaId: grupos[2].Id),

            new(empresaId, "PENSAO", "Pensão Alimentícia", "Desconto",
                tipoCalculo: "PERCENTUAL", incideInss: true,
                prioridadeDesconto: 3, ordemCalculo: 202, ordemExibicao: 102, grupoRubricaId: grupos[2].Id),

            // Descontos Voluntários
            new(empresaId, "VALE-TRANSP", "Vale Transporte 6%", "Desconto",
                tipoCalculo: "PERCENTUAL", percentual: 6, tetoMaximo: 0, // teto = salario * 0.06
                prioridadeDesconto: 10, ordemCalculo: 210, ordemExibicao: 110, grupoRubricaId: grupos[3].Id),

            new(empresaId, "VALE-REF", "Vale Refeição Coparticipação", "Desconto",
                tipoCalculo: "PERCENTUAL", percentual: 20,
                prioridadeDesconto: 11, ordemCalculo: 211, ordemExibicao: 111, grupoRubricaId: grupos[3].Id),

            new(empresaId, "PLANO-SAUDE", "Plano de Saúde Coparticipação", "Desconto",
                tipoCalculo: "VALOR_FIXO",
                prioridadeDesconto: 12, ordemCalculo: 212, ordemExibicao: 112, grupoRubricaId: grupos[3].Id),

            new(empresaId, "FALTA", "Faltas Injustificadas", "Desconto",
                tipoCalculo: "HORA", incideInss: true, incideIrrf: true,
                prioridadeDesconto: 20, ordemCalculo: 220, ordemExibicao: 120, grupoRubricaId: grupos[3].Id),

            new(empresaId, "ATRASO", "Atrasos", "Desconto",
                tipoCalculo: "HORA", incideInss: true, incideIrrf: true,
                prioridadeDesconto: 21, ordemCalculo: 221, ordemExibicao: 121, grupoRubricaId: grupos[3].Id),

            // Benefícios
            new(empresaId, "VALE-REF-PAT", "Vale Refeição Patronal", "Beneficio",
                tipoCalculo: "VALOR_FIXO", enviarEsocial: false,
                ordemCalculo: 30, ordemExibicao: 30, grupoRubricaId: grupos[4].Id),

            new(empresaId, "PLANO-SAUDE-PAT", "Plano de Saúde Patronal", "Beneficio",
                tipoCalculo: "VALOR_FIXO", enviarEsocial: false,
                ordemCalculo: 31, ordemExibicao: 31, grupoRubricaId: grupos[4].Id),

            new(empresaId, "SEGURO-VIDA", "Seguro de Vida", "Beneficio",
                tipoCalculo: "VALOR_FIXO", enviarEsocial: false,
                ordemCalculo: 32, ordemExibicao: 32, grupoRubricaId: grupos[4].Id),

            // Provisões
            new(empresaId, "PROV-13", "Provisão 13º Salário", "Provisao",
                tipoCalculo: "FORMULA", formulaCalculo: "{SALARIO_BASE} / 12", enviarEsocial: false,
                ordemCalculo: 40, ordemExibicao: 40, grupoRubricaId: grupos[5].Id),

            new(empresaId, "PROV-FERIAS", "Provisão Férias", "Provisao",
                tipoCalculo: "FORMULA", formulaCalculo: "{SALARIO_BASE} / 12", enviarEsocial: false,
                ordemCalculo: 41, ordemExibicao: 41, grupoRubricaId: grupos[5].Id),

            new(empresaId, "PROV-1-3-FERIAS", "Provisão 1/3 Férias", "Provisao",
                tipoCalculo: "FORMULA", formulaCalculo: "({SALARIO_BASE} / 12) / 3", enviarEsocial: false,
                ordemCalculo: 42, ordemExibicao: 42, grupoRubricaId: grupos[5].Id),

            new(empresaId, "PROV-MULTA-FGTS", "Provisão Multa 40% FGTS", "Provisao",
                tipoCalculo: "FORMULA", formulaCalculo: "{RUBRICA_BASE_FGTS} * 0.08 * 0.40", enviarEsocial: false,
                ordemCalculo: 43, ordemExibicao: 43, grupoRubricaId: grupos[5].Id),

            // Totais
            new(empresaId, "TOTAL-VENC", "Total Vencimentos", "Informativo",
                tipoCalculo: "COMPOSICAO", enviarEsocial: false,
                ordemCalculo: 300, ordemExibicao: 200, grupoRubricaId: grupos[7].Id),

            new(empresaId, "TOTAL-DESC", "Total Descontos", "Informativo",
                tipoCalculo: "COMPOSICAO", enviarEsocial: false,
                ordemCalculo: 301, ordemExibicao: 201, grupoRubricaId: grupos[7].Id),

            new(empresaId, "LIQUIDO", "Valor Líquido", "Informativo",
                tipoCalculo: "FORMULA", formulaCalculo: "{RUBRICA_TOTAL_VENC} - {RUBRICA_TOTAL_DESC}", enviarEsocial: false,
                ordemCalculo: 302, ordemExibicao: 202, grupoRubricaId: grupos[7].Id),
        };

        context.Set<Rubrica>().AddRange(rubricas);
        await context.SaveChangesAsync();

        // Tabelas Progressivas — IRRF 2026
        var rubricaIrrf = rubricas.First(r => r.Codigo == "IRRF");
        var tabelaIrrf = new List<RubricaTabelaProgressiva>
        {
            new(rubricaIrrf.Id, 2026, 0m, 2259.20m, 0m, 0m, 1),
            new(rubricaIrrf.Id, 2026, 2259.21m, 2826.65m, 7.5m, 169.44m, 2),
            new(rubricaIrrf.Id, 2026, 2826.66m, 3751.05m, 15m, 381.44m, 3),
            new(rubricaIrrf.Id, 2026, 3751.06m, 4664.68m, 22.5m, 662.77m, 4),
            new(rubricaIrrf.Id, 2026, 4664.69m, null, 27.5m, 896.00m, 5),
        };
        context.Set<RubricaTabelaProgressiva>().AddRange(tabelaIrrf);

        // Tabelas Progressivas — INSS 2026
        var rubricaInss = rubricas.First(r => r.Codigo == "INSS");
        var tabelaInss = new List<RubricaTabelaProgressiva>
        {
            new(rubricaInss.Id, 2026, 0m, 1412.00m, 7.5m, 0m, 1),
            new(rubricaInss.Id, 2026, 1412.01m, 2666.68m, 9m, 0m, 2),
            new(rubricaInss.Id, 2026, 2666.69m, 4000.03m, 12m, 0m, 3),
            new(rubricaInss.Id, 2026, 4000.04m, 7786.02m, 14m, 0m, 4),
        };
        context.Set<RubricaTabelaProgressiva>().AddRange(tabelaInss);

        await context.SaveChangesAsync();
        logger.LogInformation("Rubricas seeded: {RubricaCount} rubricas, {GrupoCount} grupos, {TabelaCount} tabelas progressivas",
            rubricas.Count, grupos.Count, tabelaIrrf.Count + tabelaInss.Count);
    }
}
