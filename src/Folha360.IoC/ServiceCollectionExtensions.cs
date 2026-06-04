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
        {
            var factory = sp.GetRequiredService<IDbContextFactory<Folha360DbContext>>();
            return factory.CreateDbContext();
        });

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

        return services;
    }
}
