using Folha360.WebApi.Middleware;
using Microsoft.AspNetCore.Http;

namespace Folha360.Tests.WebApi;

[Trait("Category", "Unit")]
public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task Should_GenerateCorrelationId_WhenNotPresent()
    {
        var context = new DefaultHttpContext();
        var middleware = new CorrelationIdMiddleware(async ctx =>
        {
            await Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.NotEmpty(context.Response.Headers["X-Correlation-Id"].ToString());
        Assert.NotNull(context.Items["CorrelationId"]);
    }

    [Fact]
    public async Task Should_UseExistingCorrelationId_WhenPresent()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = "existing-id-123";

        var middleware = new CorrelationIdMiddleware(async ctx =>
        {
            await Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.Equal("existing-id-123", context.Response.Headers["X-Correlation-Id"]);
        Assert.Equal("existing-id-123", context.Items["CorrelationId"]);
    }
}
