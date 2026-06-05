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
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.EnableRetryOnFailure(3))
            .AddInterceptors(new AuditInterceptor()));

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

        return services;
    }

    public static IServiceCollection AddFolha360Cadastros(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContextFactory<Folha360.Cadastros.Infrastructure.Data.CadastrosDbContext>(options =>
            options.UseNpgsql(
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

        // MediatR — registra handlers, behaviors do módulo de Cadastros
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Folha360.Cadastros.Application.Commands.CriarEmpresaCommand).Assembly);
        });

        // FluentValidation — registra validators do módulo de Cadastros
        services.AddValidatorsFromAssemblyContaining<Folha360.Cadastros.Application.Validators.CriarEmpresaCommandValidator>();

        // MassTransit + RabbitMQ
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "folha360");
                    h.Password(configuration["RabbitMQ:Password"] ?? "folha360");
                });
            });
        });

        return services;
    }
}
