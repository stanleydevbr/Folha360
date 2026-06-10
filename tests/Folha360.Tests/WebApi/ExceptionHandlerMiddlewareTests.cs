using System.Net;
using System.Text.Json;
using Folha360.WebApi.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Folha360.Tests.WebApi;

[Trait("Category", "Unit")]
public class ExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task Should_Return500_WhenExceptionThrown()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(async ctx =>
        {
            throw new Exception("Test error");
        }, loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task Should_Return401_WhenUnauthorizedAccessExceptionThrown()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(async ctx =>
        {
            throw new UnauthorizedAccessException("Not authorized");
        }, loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task Should_ReturnProblemDetailsJson()
    {
        var loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlerMiddleware(async ctx =>
        {
            throw new Exception("Test error");
        }, loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<JsonElement>(body);

        Assert.Equal("Internal Server Error", problem.GetProperty("title").GetString());
        Assert.Equal(500, problem.GetProperty("status").GetInt32());
    }
}
