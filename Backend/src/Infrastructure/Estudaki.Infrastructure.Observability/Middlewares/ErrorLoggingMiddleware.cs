using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EstudaKi.Infrastructure.Observability.Middlewares;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 500 && context.Response.StatusCode < 600)
            {
                var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                _logger.LogError(
                    "Erro HTTP {StatusCode} capturado. TraceId: {TraceId}, Path: {Path}",
                    context.Response.StatusCode,
                    traceId,
                    context.Request.Path
                );
            }
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            
            _logger.LogError(
                ex,
                "Exceção não tratada capturada. TraceId: {TraceId}, Path: {Path}, Message: {Message}",
                traceId,
                context.Request.Path,
                ex.Message
            );

            throw;
        }
    }
}

public static class ErrorLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorLoggingMiddleware>();
    }
}
