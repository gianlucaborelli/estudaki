using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Estudaki.Infrastructure.Observability.Middlewares;

public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;

    public ObservabilityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim(); // Primeiro IP é o cliente original
            }
        }

        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            return realIp.ToString();
        }

        if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
        {
            return cfIp.ToString();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;

        if (activity != null)
        {
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.path", context.Request.Path);
            activity.SetTag("http.query", context.Request.QueryString.ToString());
            activity.SetTag("http.client_ip", GetClientIpAddress(context));
            activity.SetTag("http.user_agent", context.Request.Headers.UserAgent.ToString());
            
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                activity.SetTag("user.id", context.User.Identity.Name);
                activity.SetTag("user.authenticated", true);
            }
            else
            {
                activity.SetTag("user.authenticated", false);
            }

            var requestId = context.TraceIdentifier;
            activity.SetTag("request.id", requestId);
            context.Response.Headers.Append("X-Request-ID", requestId);
        }

        await _next(context);

        if (activity != null)
        {
            activity.SetTag("http.status_code", context.Response.StatusCode);
        }
    }
}
