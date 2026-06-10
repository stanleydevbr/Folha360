using Folha360.Cadastros.Domain.Entities;
using Folha360.Domain;
using Folha360.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Folha360.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Folha360DbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Folha360DbContext>>();

        if (await context.Set<Usuario>().AnyAsync())
        {
            logger.LogInformation("Database already seeded. Skipping...");
            return;
        }

        logger.LogInformation("Seeding database...");

        // ============================
        // 1. Tenant de demonstração
        // ============================
        var tenant = new Tenant(
            tenantId: "demo",
            schemaName: "tenant_demo",
            nome: "Empresa Demo Ltda",
            status: TenantStatus.Ativo);

        context.Set<Tenant>().Add(tenant);

        // ============================
        // 2. Usuários mock para os 4 perfis
        // ============================
        var usuarios = new List<Usuario>
        {
            new(
                email: "admin@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Admin@123"),
                nome: "Administrador",
                perfil: PerfilAcesso.Admin,
                status: UsuarioStatus.Ativo),
            new(
                email: "operador@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Oper@123"),
                nome: "Operador",
                perfil: PerfilAcesso.Operador,
                status: UsuarioStatus.Ativo),
            new(
                email: "contador@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Cont@123"),
                nome: "Contador",
                perfil: PerfilAcesso.Contador,
                status: UsuarioStatus.Ativo),
            new(
                email: "consulta@folha360.com.br",
                senhaHash: PasswordHelper.HashPassword("Cons@123"),
                nome: "Consulta",
                perfil: PerfilAcesso.Consulta,
                status: UsuarioStatus.Ativo),
        };

        context.Set<Usuario>().AddRange(usuarios);
        await context.SaveChangesAsync();

        // ============================
        // 3. Empresa demo (tenant demo)
        // ============================
        var tenantGuid = TenantIdToGuid("demo");
        var empresa = new Empresa(
            tenantId: tenantGuid,
            cnpj: "00000000000191",
            razaoSocial: "Empresa Demo Ltda",
            regimeTributario: "SimplesNacional",
            nomeFantasia: "Demo",
            cnae: "6202300",
            fpas: "515",
            codigoTerceiros: "0000",
            classificacaoTributaria: "PJ",
            matrizFilial: "Matriz");

        empresa.Email = "demo@folha360.com.br";
        empresa.Telefone = "1133334444";
        empresa.EnderecoLogradouro = "Rua Exemplo";
        empresa.EnderecoNumero = "100";
        empresa.EnderecoBairro = "Centro";
        empresa.EnderecoCep = "01001000";
        empresa.EnderecoMunicipio = "São Paulo";
        empresa.EnderecoUf = "SP";

        context.Set<Empresa>().Add(empresa);

        // ============================
        // 4. Cargo padrão
        // ============================
        var cargo = new Cargo(
            empresaId: empresa.Id,
            nome: "Analista de Sistemas",
            cbo: "212405",
            descricao: "Analista de sistemas - CBO 2124-05",
            salarioBaseMinimo: 3000.00m,
            salarioBaseMaximo: 15000.00m);

        context.Set<Cargo>().Add(cargo);

        // ============================
        // 5. Lotação padrão
        // ============================
        var lotacao = new Lotacao(
            empresaId: empresa.Id,
            codigo: "TI-001",
            descricao: "Tecnologia da Informação",
            tipoEsocial: "1");

        context.Set<Lotacao>().Add(lotacao);

        // ============================
        // 6. Horário de trabalho padrão
        // ============================
        var horario = new HorarioTrabalho(
            empresaId: empresa.Id,
            codigo: "HOR-001",
            descricao: "Horário Comercial Padrão (08:00-18:00)",
            tipo: "FIXO",
            cargaHorariaDiaria: 480,
            cargaHorariaSemanal: 40,
            inicioJornada: new TimeOnly(8, 0),
            fimJornada: new TimeOnly(18, 0),
            inicioIntervalo: new TimeOnly(12, 0),
            fimIntervalo: new TimeOnly(13, 0),
            toleranciaAtrasoMinutos: 10);

        context.Set<HorarioTrabalho>().Add(horario);

        // ============================
        // 7. Funcionário demo
        // ============================
        var cpf = "52998224725";
        var cpfHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(cpf)));

        var funcionario = new Funcionario(
            empresaId: empresa.Id,
            nome: "João Silva",
            cpf: cpf,
            cpfHash: cpfHash,
            dataAdmissao: new DateOnly(2024, 1, 10),
            cargoId: cargo.Id,
            lotacaoId: lotacao.Id,
            salarioBase: 5000.00m,
            dataNascimento: new DateOnly(1990, 5, 15),
            sexo: "Masculino",
            estadoCivil: "Casado",
            nacionalidade: "Brasileiro",
            nomeMae: "Maria Silva",
            tipoContrato: "CLT",
            jornadaHorasSemanais: 40);

        funcionario.Email = "joao.silva@demo.com.br";
        funcionario.Telefone = "11988887777";
        funcionario.EnderecoLogradouro = "Av. Paulista";
        funcionario.EnderecoNumero = "1000";
        funcionario.EnderecoBairro = "Bela Vista";
        funcionario.EnderecoCep = "01310100";
        funcionario.EnderecoMunicipio = "São Paulo";
        funcionario.EnderecoUf = "SP";

        context.Set<Funcionario>().Add(funcionario);

        // ============================
        // 8. Grupo de rubricas padrão
        // ============================
        var grupoVencimentos = new GrupoRubrica(
            empresaId: empresa.Id,
            codigo: "VENCIMENTOS",
            descricao: "Vencimentos",
            natureza: "VENCIMENTO",
            ordemExibicao: 1);

        var grupoDescontos = new GrupoRubrica(
            empresaId: empresa.Id,
            codigo: "DESCONTOS",
            descricao: "Descontos",
            natureza: "DESCONTO",
            ordemExibicao: 2);

        var grupoBases = new GrupoRubrica(
            empresaId: empresa.Id,
            codigo: "BASES",
            descricao: "Bases de Cálculo",
            natureza: "BASE",
            ordemExibicao: 3);

        context.Set<GrupoRubrica>().AddRange(grupoVencimentos, grupoDescontos, grupoBases);

        // ============================
        // 9. Rubricas padrão (~30 rubricas)
        // ============================
        var rubricas = new List<Rubrica>
        {
            // --- VENCIMENTOS ---
            new(
                empresaId: empresa.Id,
                codigo: "SALARIO",
                descricao: "Salário Base",
                natureza: "VENCIMENTO",
                tipoEsocial: "1000",
                descricaoAbreviada: "Salário",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                incideDissidio: true,
                incideSalarioMaternidade: true,
                incideAuxilioDoenca: true,
                tipoCalculo: "SALARIO_BASE",
                ordemCalculo: 10,
                ordemExibicao: 1,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "HORA_EXTRA_50",
                descricao: "Horas Extras 50%",
                natureza: "VENCIMENTO",
                tipoEsocial: "1005",
                descricaoAbreviada: "HE 50%",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                incideDissidio: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 220) * {HORAS_EXTRAS_50} * 1.50",
                ordemCalculo: 20,
                ordemExibicao: 2,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "HORA_EXTRA_100",
                descricao: "Horas Extras 100%",
                natureza: "VENCIMENTO",
                tipoEsocial: "1006",
                descricaoAbreviada: "HE 100%",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 220) * {HORAS_EXTRAS_100} * 2.00",
                ordemCalculo: 30,
                ordemExibicao: 3,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "DSR",
                descricao: "Descanso Semanal Remunerado",
                natureza: "VENCIMENTO",
                tipoEsocial: "1010",
                descricaoAbreviada: "DSR",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({HORA_EXTRA_50_VALOR} + {HORA_EXTRA_100_VALOR}) / {DIAS_UTEIS} * {DOMINGOS_FERIADOS}",
                ordemCalculo: 40,
                ordemExibicao: 4,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "ADIC_NOTURNO",
                descricao: "Adicional Noturno",
                natureza: "VENCIMENTO",
                tipoEsocial: "1007",
                descricaoAbreviada: "Ad.Noturno",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 220) * {HORAS_NOTURNAS} * 0.20",
                ordemCalculo: 50,
                ordemExibicao: 5,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "ADIC_INSALUBRIDADE",
                descricao: "Adicional de Insalubridade",
                natureza: "VENCIMENTO",
                tipoEsocial: "1008",
                descricaoAbreviada: "Insalub.",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "PERCENTUAL_SALARIO_MINIMO",
                percentual: 20.00m,
                ordemCalculo: 60,
                ordemExibicao: 6,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "ADIC_PERI",
                descricao: "Adicional de Periculosidade",
                natureza: "VENCIMENTO",
                tipoEsocial: "1009",
                descricaoAbreviada: "Pericul.",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "PERCENTUAL",
                percentual: 30.00m,
                ordemCalculo: 70,
                ordemExibicao: 7,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "COMISSAO",
                descricao: "Comissões",
                natureza: "VENCIMENTO",
                tipoEsocial: "1011",
                descricaoAbreviada: "Comissão",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "VALOR_FIXO",
                ordemCalculo: 80,
                ordemExibicao: 8,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "SALARIO_FAMILIA",
                descricao: "Salário Família",
                natureza: "VENCIMENTO",
                tipoEsocial: "1012",
                descricaoAbreviada: "Sal.Família",
                enviarEsocial: true,
                incideFgts: true,
                tipoCalculo: "SALARIO_FAMILIA",
                ordemCalculo: 90,
                ordemExibicao: 9,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "ADIANTAMENTO",
                descricao: "Adiantamento Salarial",
                natureza: "VENCIMENTO",
                tipoEsocial: "1013",
                descricaoAbreviada: "Adiant.",
                enviarEsocial: true,
                incideAdiantamento: true,
                tipoCalculo: "VALOR_FIXO",
                ordemCalculo: 100,
                ordemExibicao: 10,
                grupoRubricaId: grupoVencimentos.Id),

            // --- DESCONTOS ---
            new(
                empresaId: empresa.Id,
                codigo: "INSS",
                descricao: "INSS — Contribuição Previdenciária",
                natureza: "DESCONTO",
                tipoEsocial: "2000",
                descricaoAbreviada: "INSS",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                incideSalarioMaternidade: true,
                tipoCalculo: "TABELA_PROGRESSIVA",
                ordemCalculo: 200,
                ordemExibicao: 20,
                prioridadeDesconto: 1,
                tetoMaximo: 908.85m,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "IRRF",
                descricao: "IRRF — Imposto de Renda Retido na Fonte",
                natureza: "DESCONTO",
                tipoEsocial: "2100",
                descricaoAbreviada: "IRRF",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "TABELA_PROGRESSIVA",
                ordemCalculo: 210,
                ordemExibicao: 21,
                prioridadeDesconto: 2,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "FGTS",
                descricao: "FGTS — Fundo de Garantia",
                natureza: "DESCONTO",
                tipoEsocial: "2200",
                descricaoAbreviada: "FGTS",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                incideAvisoPrevio: true,
                incideRescisao: true,
                tipoCalculo: "PERCENTUAL",
                percentual: 8.00m,
                ordemCalculo: 220,
                ordemExibicao: 22,
                prioridadeDesconto: 3,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "VALE_TRANSPORTE",
                descricao: "Vale Transporte",
                natureza: "DESCONTO",
                tipoEsocial: "2300",
                descricaoAbreviada: "VT",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                tipoCalculo: "PERCENTUAL",
                percentual: 6.00m,
                tetoMaximo: 300.00m,
                ordemCalculo: 230,
                ordemExibicao: 23,
                prioridadeDesconto: 4,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "VALE_REFEICAO",
                descricao: "Vale Refeição",
                natureza: "DESCONTO",
                tipoEsocial: "2400",
                descricaoAbreviada: "VR",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                tipoCalculo: "VALOR_FIXO",
                valorFixo: 150.00m,
                ordemCalculo: 240,
                ordemExibicao: 24,
                prioridadeDesconto: 5,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "PENSAO_ALIMENTICIA",
                descricao: "Pensão Alimentícia",
                natureza: "DESCONTO",
                tipoEsocial: "2500",
                descricaoAbreviada: "Pensão",
                enviarEsocial: true,
                tipoCalculo: "PERCENTUAL",
                percentual: 20.00m,
                ordemCalculo: 250,
                ordemExibicao: 25,
                prioridadeDesconto: 6,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "CONTRIB_SINDICAL",
                descricao: "Contribuição Sindical",
                natureza: "DESCONTO",
                tipoEsocial: "2600",
                descricaoAbreviada: "Sindical",
                enviarEsocial: true,
                incideContribuicaoSindical: true,
                tipoCalculo: "VALOR_FIXO",
                valorFixo: 50.00m,
                ordemCalculo: 260,
                ordemExibicao: 26,
                prioridadeDesconto: 7,
                grupoRubricaId: grupoDescontos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "FALTAS",
                descricao: "Faltas e Atrasos",
                natureza: "DESCONTO",
                tipoEsocial: "2700",
                descricaoAbreviada: "Faltas",
                enviarEsocial: true,
                incideDecimoTerceiro: true,
                incideFerias: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 30) * {DIAS_FALTA}",
                ordemCalculo: 270,
                ordemExibicao: 27,
                prioridadeDesconto: 8,
                grupoRubricaId: grupoDescontos.Id),

            // --- FÉRIAS ---
            new(
                empresaId: empresa.Id,
                codigo: "FERIAS",
                descricao: "Férias",
                natureza: "VENCIMENTO",
                tipoEsocial: "3000",
                descricaoAbreviada: "Férias",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                tipoCalculo: "FERIAS",
                ordemCalculo: 300,
                ordemExibicao: 30,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "FERIAS_1_3",
                descricao: "1/3 Constitucional de Férias",
                natureza: "VENCIMENTO",
                tipoEsocial: "3010",
                descricaoAbreviada: "1/3 Férias",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "{FERIAS_VALOR} / 3",
                ordemCalculo: 310,
                ordemExibicao: 31,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "ABONO_PECUNIARIO",
                descricao: "Abono Pecuniário de Férias",
                natureza: "VENCIMENTO",
                tipoEsocial: "3020",
                descricaoAbreviada: "Abono Férias",
                enviarEsocial: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 30) * {DIAS_ABONO}",
                ordemCalculo: 320,
                ordemExibicao: 32,
                grupoRubricaId: grupoVencimentos.Id),

            // --- 13º SALÁRIO ---
            new(
                empresaId: empresa.Id,
                codigo: "DECIMO_TERCEIRO",
                descricao: "13º Salário",
                natureza: "VENCIMENTO",
                tipoEsocial: "4000",
                descricaoAbreviada: "13º",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                tipoCalculo: "DECIMO_TERCEIRO",
                ordemCalculo: 400,
                ordemExibicao: 40,
                grupoRubricaId: grupoVencimentos.Id),

            // --- RESCISÃO ---
            new(
                empresaId: empresa.Id,
                codigo: "SALDO_SALARIO",
                descricao: "Saldo de Salário (Rescisão)",
                natureza: "VENCIMENTO",
                tipoEsocial: "5000",
                descricaoAbreviada: "Saldo Sal.",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "({SALARIO_BASE} / 30) * {DIAS_TRABALHADOS}",
                ordemCalculo: 500,
                ordemExibicao: 50,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "AVISO_PREVIO",
                descricao: "Aviso Prévio",
                natureza: "VENCIMENTO",
                tipoEsocial: "5010",
                descricaoAbreviada: "Aviso Prévio",
                enviarEsocial: true,
                incideInss: true,
                incideIrrf: true,
                incideFgts: true,
                tipoCalculo: "AVISO_PREVIO",
                ordemCalculo: 510,
                ordemExibicao: 51,
                grupoRubricaId: grupoVencimentos.Id),

            new(
                empresaId: empresa.Id,
                codigo: "MULTA_FGTS",
                descricao: "Multa de 40% FGTS (Rescisão)",
                natureza: "VENCIMENTO",
                tipoEsocial: "5020",
                descricaoAbreviada: "Multa FGTS",
                enviarEsocial: true,
                tipoCalculo: "FORMULA",
                formulaCalculo: "{FGTS_SALDO} * 0.40",
                ordemCalculo: 520,
                ordemExibicao: 52,
                grupoRubricaId: grupoVencimentos.Id),

            // --- BASES ---
            new(
                empresaId: empresa.Id,
                codigo: "BASE_INSS",
                descricao: "Base de Cálculo INSS",
                natureza: "BASE",
                tipoEsocial: "9000",
                descricaoAbreviada: "Base INSS",
                enviarEsocial: false,
                tipoCalculo: "BASE",
                ordemCalculo: 150,
                ordemExibicao: 99,
                grupoRubricaId: grupoBases.Id),

            new(
                empresaId: empresa.Id,
                codigo: "BASE_IRRF",
                descricao: "Base de Cálculo IRRF",
                natureza: "BASE",
                tipoEsocial: "9001",
                descricaoAbreviada: "Base IRRF",
                enviarEsocial: false,
                tipoCalculo: "BASE",
                ordemCalculo: 160,
                ordemExibicao: 99,
                grupoRubricaId: grupoBases.Id),

            new(
                empresaId: empresa.Id,
                codigo: "BASE_FGTS",
                descricao: "Base de Cálculo FGTS",
                natureza: "BASE",
                tipoEsocial: "9002",
                descricaoAbreviada: "Base FGTS",
                enviarEsocial: false,
                tipoCalculo: "BASE",
                ordemCalculo: 170,
                ordemExibicao: 99,
                grupoRubricaId: grupoBases.Id),
        };

        context.Set<Rubrica>().AddRange(rubricas);

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Database seeded successfully: {UserCount} users, 1 tenant, 1 empresa, 1 cargo, 1 lotacao, 1 horario, 1 funcionario, 3 grupos, {RubricaCount} rubricas",
            usuarios.Count,
            rubricas.Count);
    }

    /// <summary>
    /// Converte TenantId string (ex.: "demo") em GUID determinístico via MD5.
    /// </summary>
    private static Guid TenantIdToGuid(string tenantId)
    {
        var hash = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(tenantId));
        return new Guid(hash);
    }
}
