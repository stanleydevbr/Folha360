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
            .AddCheck<Infrastructure.HealthChecks.RabbitMqHealthCheck>("rabbitmq");

        // Cadastros Module (F02)
        services.AddFolha360Cadastros(configuration);

        // Eventos Trabalhistas Module (F03)
        services.AddFolha360Eventos(configuration);

        // Processamento da Folha Module (F04)
        services.AddFolha360Processamento(configuration);

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
