using Folha360.Infrastructure.MultiTenancy;

namespace Folha360.WebApi.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TenantResolutionStrategy tenantResolver)
    {
        await tenantResolver.ResolveAsync(context.RequestAborted);
        await _next(context);
    }
}
