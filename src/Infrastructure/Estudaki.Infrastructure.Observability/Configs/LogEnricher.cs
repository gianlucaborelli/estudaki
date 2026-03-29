using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace Estudaki.Infrastructure.Observability.Configs;

internal class LogEnricher : BaseProcessor<LogRecord>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public LogEnricher(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void OnEnd(LogRecord logRecord)
    {
        Enrich(logRecord);
        base.OnEnd(logRecord);
    }

    private void Enrich(LogRecord logRecord)
    {
        var activity = Activity.Current;

        if (activity != null)
        {
            var attributes = new List<KeyValuePair<string, object?>>
            {
                new("TraceId", activity.TraceId.ToString()),
                new("SpanId", activity.SpanId.ToString())
            };

            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null)
            {
                attributes.Add(new("http.method", httpContext.Request.Method));
                attributes.Add(new("http.path", httpContext.Request.Path.ToString()));
                attributes.Add(new("http.user_agent", httpContext.Request.Headers.UserAgent.ToString()));
                var ip = httpContext.Connection.RemoteIpAddress?.ToString();
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].ToString();
                attributes.Add(new("client.ip", ip ?? "unknown"));

                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    attributes.Add(new("client.forwarded_for", forwardedFor));
                }

                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    attributes.Add(new("user.id", httpContext.User.Identity.Name ?? "unknown"));
                }
            }

            // Informações da máquina
            attributes.Add(new("host.name", Environment.MachineName));
            attributes.Add(new("environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"));

            if (logRecord.Attributes != null)
            {
                attributes.AddRange(logRecord.Attributes);
            }

            logRecord.Attributes = attributes;
        }
    }
}
