using System.Net;
using Folha360.Application.DTOs;

namespace Folha360.WebApi.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access for {CorrelationId}",
                context.Items["CorrelationId"]);

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetailsResponse(
                Type: "https://tools.ietf.org/html/rfc7807",
                Title: "Unauthorized",
                Status: 401,
                Detail: ex.Message,
                Instance: context.Request.Path,
                Errors: null);

            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {CorrelationId}",
                context.Items["CorrelationId"]);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetailsResponse(
                Type: "https://tools.ietf.org/html/rfc7807",
                Title: "Internal Server Error",
                Status: 500,
                Detail: "An unexpected error occurred.",
                Instance: context.Request.Path,
                Errors: null);

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
