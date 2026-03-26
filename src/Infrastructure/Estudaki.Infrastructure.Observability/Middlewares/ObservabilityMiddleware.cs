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

    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        
        if (activity != null)
        {
            // Enriquece com informações do request
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.path", context.Request.Path);
            activity.SetTag("http.query", context.Request.QueryString.ToString());
            activity.SetTag("http.client_ip", context.Connection.RemoteIpAddress?.ToString());
            activity.SetTag("http.user_agent", context.Request.Headers.UserAgent.ToString());
            
            // User info
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                activity.SetTag("user.id", context.User.Identity.Name);
                activity.SetTag("user.authenticated", true);
            }
            else
            {
                activity.SetTag("user.authenticated", false);
            }

            // Request ID
            var requestId = context.TraceIdentifier;
            activity.SetTag("request.id", requestId);
            context.Response.Headers.Append("X-Request-ID", requestId);
        }

        await _next(context);

        // Enriquece com informações do response
        if (activity != null)
        {
            activity.SetTag("http.status_code", context.Response.StatusCode);
        }
    }
}
