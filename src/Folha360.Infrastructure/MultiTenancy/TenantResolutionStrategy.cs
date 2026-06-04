using Folha360.Domain.Abstractions;
using Folha360.Domain.Entities;
using Folha360.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Infrastructure.MultiTenancy;

public class TenantResolutionStrategy : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContextFactory<Folha360DbContext> _dbContextFactory;
    private readonly AsyncLocal<TenantInfo> _cachedTenant = new();

    public string TenantId => _cachedTenant.Value?.TenantId ?? string.Empty;
    public string SchemaName => _cachedTenant.Value?.SchemaName ?? string.Empty;

    public TenantResolutionStrategy(
        IHttpContextAccessor httpContextAccessor,
        IDbContextFactory<Folha360DbContext> dbContextFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContextFactory = dbContextFactory;
    }

    public async Task ResolveAsync(CancellationToken ct = default)
    {
        if (_cachedTenant.Value != null)
            return;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        var tenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (string.IsNullOrEmpty(tenantId))
        {
            // Tentar obter do claim JWT
            tenantId = httpContext.User?.FindFirst("tenant_id")?.Value;
        }

        if (string.IsNullOrEmpty(tenantId))
        {
            _cachedTenant.Value = new TenantInfo
            {
                TenantId = string.Empty,
                SchemaName = "public"
            };
            return;
        }

        // Cachear no HttpContext.Items para evitar múltiplas resoluções
        const string cacheKey = "Folha360_TenantInfo";
        if (httpContext.Items[cacheKey] is TenantInfo cached)
        {
            _cachedTenant.Value = cached;
            return;
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var tenant = await context.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.TenantId == tenantId, ct);

        var tenantInfo = tenant != null
            ? new TenantInfo { TenantId = tenant.TenantId, SchemaName = tenant.SchemaName }
            : new TenantInfo { TenantId = tenantId, SchemaName = $"tenant_{tenantId}" };

        _cachedTenant.Value = tenantInfo;
        httpContext.Items[cacheKey] = tenantInfo;
    }

    private class TenantInfo
    {
        public string TenantId { get; init; } = string.Empty;
        public string SchemaName { get; init; } = string.Empty;
    }
}
