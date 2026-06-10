using System.Text;
using Folha360.Application.Commands;
using Folha360.Application.Services;
using Folha360.Domain.Abstractions;
using Folha360.Infrastructure.Data;
using Folha360.Infrastructure.HealthChecks;
using Folha360.Infrastructure.MultiTenancy;
using Folha360.Infrastructure.Repositories;
using FluentValidation;
using Folha360.Infrastructure.Services;
using Folha360.Processamento.Application.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;

namespace Folha360.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFolha360Infrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360DbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3))
            .AddInterceptors(new AuditInterceptor())
            .ConfigureWarnings(w => w.Ignore(
                Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

        services.AddScoped<Folha360DbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360DbContext>>().CreateDbContext());

        // Multi-tenancy
        services.AddHttpContextAccessor();
        services.AddScoped<TenantResolutionStrategy>();
        services.AddScoped<ITenantContext>(sp =>
            sp.GetRequiredService<TenantResolutionStrategy>());

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // Services
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHealthCheckService, AppHealthCheckService>();
        services.AddMemoryCache();

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

        // JWT Authentication
        var jwtSecret = configuration["Jwt:Secret"] ?? "Folha360@DevSecretKey@2026!TempKey";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "Folha360";
        var jwtAudience = configuration["Jwt:Audience"] ?? "Folha360.Api";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));
            options.AddPolicy("Operador", policy =>
                policy.RequireRole("Admin", "Operador"));
            options.AddPolicy("Contador", policy =>
                policy.RequireRole("Admin", "Contador"));
            options.AddPolicy("Consulta", policy =>
                policy.RequireRole("Admin", "Operador", "Contador", "Consulta"));
        });

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<Infrastructure.HealthChecks.PostgresqlHealthCheck>("postgresql")
            .AddCheck<Infrastructure.HealthChecks.RedisHealthCheck>("redis")
            .AddCheck<Infrastructure.HealthChecks.RabbitMqHealthCheck>("rabbitmq")
            .AddCheck<Folha360.Relatorios.Infrastructure.Health.ReadReplicaHealthCheck>("read_replica");

        // Cadastros Module (F02)
        services.AddFolha360Cadastros(configuration);

        // Eventos Trabalhistas Module (F03)
        services.AddFolha360Eventos(configuration);

        // Processamento da Folha Module (F04)
        services.AddFolha360Processamento(configuration);

        // Obrigações Fiscais Module (F05)
        services.AddFolha360Fiscais(configuration);

        // Relatórios & Exportações Module (F06)
        services.AddFolha360Relatorios(configuration);

        // MassTransit + RabbitMQ (centralizado — único AddMassTransit por container)
        services.AddFolha360MassTransit(configuration);

        return services;
    }

    public static IServiceCollection AddFolha360Cadastros(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Cadastros.Infrastructure.Data.CadastrosDbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<Folha360.Cadastros.Infrastructure.Data.CadastrosDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360.Cadastros.Infrastructure.Data.CadastrosDbContext>>().CreateDbContext());

        // Repositories
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IEmpresaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.EmpresaRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IFuncionarioRepository,
            Folha360.Cadastros.Infrastructure.Repositories.FuncionarioRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.ICargoRepository,
            Folha360.Cadastros.Infrastructure.Repositories.CargoRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.ILotacaoRepository,
            Folha360.Cadastros.Infrastructure.Repositories.LotacaoRepository>();

        // Subsistema de Rubricas (ADR-006) — Repositories
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IGrupoRubricaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.GrupoRubricaRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaComposicaoRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaComposicaoRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaFormulaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaFormulaRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaTabelaProgressivaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaTabelaProgressivaRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaHistoricoRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaHistoricoRepository>();
        services.AddScoped<Folha360.Cadastros.Domain.Abstractions.IRubricaIncidenciaRepository,
            Folha360.Cadastros.Infrastructure.Repositories.RubricaIncidenciaRepository>();

        // MediatR — registra handlers, behaviors do módulo de Cadastros
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Cadastros.Application.Commands.CriarEmpresaCommand).Assembly);
        });

        // FluentValidation — registra validators do módulo de Cadastros
        services.AddValidatorsFromAssemblyContaining<Folha360.Cadastros.Application.Validators.CriarEmpresaCommandValidator>();

        // Domain Services — Motor de Cálculo (ADR-006)
        services.AddScoped<Folha360.Cadastros.Domain.Services.ResolvedorComposicao>();
        services.AddScoped<Folha360.Cadastros.Domain.Services.AplicadorTabelaProgressiva>();
        services.AddScoped<Folha360.Cadastros.Domain.Services.CalculadorMedia>();
        services.AddScoped<Folha360.Cadastros.Domain.Services.AvaliadorCondicional>();
        services.AddScoped<Folha360.Cadastros.Domain.Services.IExpressionEvaluator,
            Folha360.Cadastros.Infrastructure.Services.NCalcExpressionEvaluator>();
        services.AddScoped<Folha360.Cadastros.Domain.Services.MotorCalculo>();

        return services;
    }

    public static IServiceCollection AddFolha360Eventos(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Eventos.Infrastructure.Data.EventosDbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<Folha360.Eventos.Infrastructure.Data.EventosDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360.Eventos.Infrastructure.Data.EventosDbContext>>().CreateDbContext());

        // Repositories
        services.AddScoped<Folha360.Eventos.Domain.Abstractions.IAdmissaoRepository,
            Folha360.Eventos.Infrastructure.Repositories.AdmissaoRepository>();
        services.AddScoped<Folha360.Eventos.Domain.Abstractions.IFeriasRepository,
            Folha360.Eventos.Infrastructure.Repositories.FeriasRepository>();
        services.AddScoped<Folha360.Eventos.Domain.Abstractions.IAfastamentoRepository,
            Folha360.Eventos.Infrastructure.Repositories.AfastamentoRepository>();
        services.AddScoped<Folha360.Eventos.Domain.Abstractions.IDesligamentoRepository,
            Folha360.Eventos.Infrastructure.Repositories.DesligamentoRepository>();
        services.AddScoped<Folha360.Eventos.Domain.Abstractions.IAlteracaoContratualRepository,
            Folha360.Eventos.Infrastructure.Repositories.AlteracaoContratualRepository>();

        // Services
        services.AddScoped<Folha360.Eventos.Application.Services.IXmlGeradorService,
            Folha360.Eventos.Infrastructure.Services.XmlGeradorService>();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Eventos.Application.Commands.CriarAdmissaoCommand).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Folha360.Eventos.Application.Validators.CriarAdmissaoCommandValidator>();

        return services;
    }

    public static IServiceCollection AddFolha360Processamento(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Processamento.Infrastructure.Data.ProcessamentoDbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<Folha360.Processamento.Infrastructure.Data.ProcessamentoDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360.Processamento.Infrastructure.Data.ProcessamentoDbContext>>().CreateDbContext());

        // Repositories
        services.AddScoped<Folha360.Processamento.Domain.Abstractions.IProcessamentoRepository,
            Folha360.Processamento.Infrastructure.Repositories.ProcessamentoRepository>();
        services.AddScoped<Folha360.Processamento.Domain.Abstractions.IItemFolhaRepository,
            Folha360.Processamento.Infrastructure.Repositories.ItemFolhaRepository>();
        services.AddScoped<Folha360.Processamento.Domain.Abstractions.IHoleriteRepository,
            Folha360.Processamento.Infrastructure.Repositories.HoleriteRepository>();
        services.AddScoped<Folha360.Processamento.Domain.Abstractions.ICadeiaFechamentoRepository,
            Folha360.Processamento.Infrastructure.Repositories.CadeiaFechamentoRepository>();

        // Domain Services — Motor de Cálculo (F04)
        services.AddScoped<Folha360.Processamento.Domain.Services.IAvaliadorExpressao,
            Folha360.Processamento.Domain.Services.AvaliadorExpressao>();
        services.AddScoped<Folha360.Processamento.Domain.Services.IResolvedorComposicao,
            Folha360.Processamento.Domain.Services.ResolvedorComposicao>();
        services.AddScoped<Folha360.Processamento.Domain.Services.IAplicadorTabelaProgressiva,
            Folha360.Processamento.Domain.Services.AplicadorTabelaProgressiva>();
        services.AddScoped<Folha360.Processamento.Domain.Services.ICalculadorMedia,
            Folha360.Processamento.Domain.Services.CalculadorMedia>();
        services.AddScoped<Folha360.Processamento.Domain.Services.IAvaliadorCondicional,
            Folha360.Processamento.Domain.Services.AvaliadorCondicional>();
        services.AddScoped<Folha360.Processamento.Domain.Services.IMotorCalculo,
            Folha360.Processamento.Domain.Services.MotorCalculo>();

        // Redis
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(
                _ => StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnectionString));
        }

        // Infrastructure Services
        services.AddScoped<Folha360.Processamento.Domain.Services.IExpressionEvaluator,
            Folha360.Processamento.Infrastructure.Services.NCalcExpressionEvaluator>();
        services.AddScoped<IRedisCacheService,
            Folha360.Processamento.Infrastructure.Services.RedisCacheService>();
        services.AddScoped<IPdfGeradorService,
            Folha360.Processamento.Infrastructure.Services.PdfGeradorService>();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Processamento.Application.Commands.IniciarProcessamentoCommand).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Folha360.Processamento.Application.Validators.IniciarProcessamentoCommandValidator>();

        // SignalR
        services.AddSignalR();

        return services;
    }

    public static IServiceCollection AddFolha360Fiscais(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Fiscais.Infrastructure.Data.FiscaisDbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<Folha360.Fiscais.Infrastructure.Data.FiscaisDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360.Fiscais.Infrastructure.Data.FiscaisDbContext>>().CreateDbContext());

        // Repositories
        services.AddScoped<Folha360.Fiscais.Domain.Abstractions.IApuracaoFiscalRepository,
            Folha360.Fiscais.Infrastructure.Repositories.ApuracaoFiscalRepository>();
        services.AddScoped<Folha360.Fiscais.Domain.Abstractions.IGuiaRecolhimentoRepository,
            Folha360.Fiscais.Infrastructure.Repositories.GuiaRecolhimentoRepository>();
        services.AddScoped<Folha360.Fiscais.Domain.Abstractions.IRegraFiscalRepository,
            Folha360.Fiscais.Infrastructure.Repositories.RegraFiscalRepository>();
        services.AddScoped<Folha360.Fiscais.Domain.Abstractions.ILancamentoContabilRepository,
            Folha360.Fiscais.Infrastructure.Repositories.LancamentoContabilRepository>();

        // Domain Services — Strategy Pattern (8 implementações)
        services.AddScoped<Folha360.Fiscais.Domain.Services.IrpfRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.InssRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.FgtsRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.PisRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.CofinsRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.CsllRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.SindicalRegraFiscalService>();
        services.AddScoped<Folha360.Fiscais.Domain.Services.IssRegraFiscalService>();

        // Factory
        services.AddSingleton<Folha360.Fiscais.Domain.Abstractions.IRegraFiscalFactory>(sp =>
        {
            var dict = new Dictionary<Folha360.Fiscais.Domain.Tributo, Folha360.Fiscais.Domain.Abstractions.IRegraFiscalService>
            {
                [Folha360.Fiscais.Domain.Tributo.IRRF] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.IrpfRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.INSS] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.InssRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.FGTS] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.FgtsRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.PIS] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.PisRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.COFINS] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.CofinsRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.CSLL] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.CsllRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.ContribuicaoSindical] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.SindicalRegraFiscalService>(),
                [Folha360.Fiscais.Domain.Tributo.ISS] = sp.GetRequiredService<Folha360.Fiscais.Domain.Services.IssRegraFiscalService>(),
            };
            return new Folha360.Fiscais.Domain.Services.RegraFiscalFactory(dict);
        });

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Fiscais.Application.Commands.ApurarObrigacoesCommand).Assembly);
        });

        // Infrastructure Services — PDF, CSV/SPED, SFTP, Redis Cache
        services.AddScoped<Folha360.Fiscais.Application.Services.IGeradorGuiaPdfService,
            Folha360.Fiscais.Infrastructure.Services.GeradorGuiaPdfService>();
        services.AddScoped<Folha360.Fiscais.Application.Services.IExportadorContabilService,
            Folha360.Fiscais.Infrastructure.Services.ExportadorContabilService>();
        services.AddScoped<Folha360.Fiscais.Application.Services.ISftpService,
            Folha360.Fiscais.Infrastructure.Services.SftpService>();
        services.AddSingleton<Folha360.Fiscais.Application.Services.IRedisCacheService,
            Folha360.Fiscais.Infrastructure.Services.RedisCacheService>();

        return services;
    }

    public static IServiceCollection AddFolha360Relatorios(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Relatorios.Infrastructure.Data.RelatoriosDbContext>(options =>
            options.UseSnakeCaseNamingConvention()
            .UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<Folha360.Relatorios.Infrastructure.Data.RelatoriosDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<Folha360.Relatorios.Infrastructure.Data.RelatoriosDbContext>>().CreateDbContext());

        // Repositories
        services.AddScoped<Folha360.Relatorios.Domain.Abstractions.IRelatorioRepository,
            Folha360.Relatorios.Infrastructure.Repositories.RelatorioRepository>();
        services.AddScoped<Folha360.Relatorios.Domain.Abstractions.IAgendamentoRepository,
            Folha360.Relatorios.Infrastructure.Repositories.AgendamentoRepository>();

        // Domain Services — Storage & Cache
        services.AddScoped<Folha360.Relatorios.Domain.Abstractions.IRelatorioStorageService,
            Folha360.Relatorios.Infrastructure.Services.RelatorioStorageService>();
        services.AddSingleton<Folha360.Relatorios.Application.Services.IRedisCacheService,
            Folha360.Relatorios.Infrastructure.Services.RedisCacheService>();

        // Application Services — PDF, Export, Email, Agendamento
        services.AddScoped<Folha360.Relatorios.Application.Services.IRelatorioPdfService,
            Folha360.Relatorios.Infrastructure.Services.RelatorioPdfService>();
        services.AddScoped<Folha360.Relatorios.Application.Services.IRelatorioExportService,
            Folha360.Relatorios.Infrastructure.Services.RelatorioExportService>();
        services.AddScoped<Folha360.Relatorios.Application.Services.IRelatorioEmailService,
            Folha360.Relatorios.Infrastructure.Services.RelatorioEmailService>();
        services.AddScoped<Folha360.Relatorios.Application.Services.IAgendamentoService,
            Folha360.Relatorios.Infrastructure.Services.AgendamentoService>();

        // Metrics
        services.AddSingleton<Folha360.Relatorios.Infrastructure.Metrics.RelatoriosMetrics>();

        // Quartz.NET
        services.AddQuartz(q =>
        {
            q.UsePersistentStore(store =>
            {
                store.UsePostgres(postgres =>
                {
                    postgres.ConnectionString = configuration.GetConnectionString("Postgres")!;
                });
            });

            var jobKey = new JobKey("gerar_relatorio_job");
            q.AddJob<Folha360.Relatorios.Infrastructure.Jobs.GerarRelatorioJob>(jobKey, j => j
                .StoreDurably()
                .WithDescription("Job de geração de relatórios agendados"));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Relatorios.Application.Commands.GerarHoleritesLoteCommand).Assembly);
        });

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<Folha360.Relatorios.Application.Validators.GerarHoleritesLoteCommandValidator>();

        return services;
    }

    public static IServiceCollection AddFolha360MassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Consumers — Módulo F03
            x.AddConsumer<Folha360.Eventos.Application.Consumers.FuncionarioCadastradoConsumer>();
            x.AddConsumer<Folha360.Eventos.Application.Consumers.GerarXmlAdmissaoConsumer>();
            x.AddConsumer<Folha360.Eventos.Application.Consumers.GerarXmlFeriasConsumer>();
            x.AddConsumer<Folha360.Eventos.Application.Consumers.GerarXmlAfastamentoConsumer>();
            x.AddConsumer<Folha360.Eventos.Application.Consumers.GerarXmlDesligamentoConsumer>();
            x.AddConsumer<Folha360.Eventos.Application.Consumers.GerarXmlAlteracaoContratualConsumer>();

            // Consumers — Módulo F04 (Processamento)
            x.AddConsumer<Folha360.Processamento.Application.Consumers.ProcessarFolhaConsumer>();
            x.AddConsumer<Folha360.Processamento.Application.Consumers.RubricaAlteradaConsumer>();
            x.AddConsumer<Folha360.Processamento.Application.Consumers.ReaberturaSolicitadaConsumer>();

            // Consumers — Módulo F05 (Obrigações Fiscais)
            x.AddConsumer<Folha360.Fiscais.Application.Consumers.ApurarObrigacoesFiscaisConsumer>();
            x.AddConsumer<Folha360.Fiscais.Application.Consumers.ReverterObrigacoesConsumer>();
            x.AddConsumer<Folha360.Fiscais.Application.Consumers.FolhaReabertaConsumer>();

            // Consumers — Módulo F06 (Relatórios & Exportações)
            x.AddConsumer<Folha360.Relatorios.Application.Consumers.FolhaFechadaConsumer>();
            x.AddConsumer<Folha360.Relatorios.Application.Consumers.ObrigacoesApuradasConsumer>();
            x.AddConsumer<Folha360.Relatorios.Application.Consumers.FolhaReabertaConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "folha360");
                    h.Password(configuration["RabbitMQ:Password"] ?? "folha360");
                });

                cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2)));
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
